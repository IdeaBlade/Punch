// ====================================================================================================================
//   Copyright (c) 2012 IdeaBlade
// ====================================================================================================================
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//   OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//   OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// ====================================================================================================================
//   USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
//   http://cocktail.ideablade.com/licensing
// ====================================================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using IdeaBlade.EntityModel;
using IdeaBlade.EntityModel.Security;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Cocktail
{
    /// <summary>
    ///     SuspensionManager captures global session state to simplify process lifetime management
    ///     for an application.  Note that session state will be automatically cleared under a variety
    ///     of conditions and should only be used to store information that would be convenient to
    ///     carry across sessions, but that should be discarded when an application crashes or is
    ///     upgraded.
    /// </summary>
    internal sealed class SuspensionManager
    {
        private const string SessionStateFilename = "_sessionState.xml";
        private static Dictionary<string, object> _sessionState = new Dictionary<string, object>();

        private static readonly DependencyProperty FrameSessionStateKeyProperty =
            DependencyProperty.RegisterAttached("_FrameSessionStateKey", typeof(String), typeof(SuspensionManager), null);

        private static readonly DependencyProperty FrameSessionStateProperty =
            DependencyProperty.RegisterAttached("_FrameSessionState", typeof(Dictionary<String, Object>),
                                                typeof(SuspensionManager), null);

        private static readonly List<WeakReference<Frame>> RegisteredFrames = new List<WeakReference<Frame>>();
        private static List<Type> _knownTypes;

        private static readonly PartLocator<IAuthenticationService> AuthenticationServiceLocator =
            new PartLocator<IAuthenticationService>();

        private static readonly PartLocator<INavigator> RootNavigatorLocator =
            new PartLocator<INavigator>();

        /// <summary>
        ///     Provides access to global session state for the current session.  This state is
        ///     serialized by <see cref="SaveAsync" /> and restored by
        ///     <see cref="RestoreAsync" />, so values must be serializable by
        ///     <see cref="DataContractSerializer" /> and should be as compact as possible.  Strings
        ///     and other self-contained data types are strongly recommended.
        /// </summary>
        public static Dictionary<string, object> SessionState
        {
            get { return _sessionState; }
        }

        private static IEnumerable<Type> KnownTypes
        {
            get
            {
                return _knownTypes ?? (_knownTypes = new List<Type>(KnownTypeHelper.GetServiceKnownTypes())
                                                         {
                                                             typeof(SerializableAuthenticationContext),
                                                             typeof(SerializableLoginOptions),
                                                             typeof(UserBase)
                                                         });
            }
        }

        public static bool AutomaticSessionRestoreEnabled { get; set; }

        /// <summary>
        ///     Save the current <see cref="SessionState" />.  Any <see cref="Frame" /> instances
        ///     registered with <see cref="RegisterFrame" /> will also preserve their current
        ///     navigation stack, which in turn gives their active <see cref="Page" /> an opportunity
        ///     to save its state.
        /// </summary>
        /// <returns>An asynchronous task that reflects when session state has been saved.</returns>
        public static async Task SaveAsync()
        {
            try
            {
                // Notify Root Navigator and AuthenticationService that we are suspending
                if (RootNavigatorLocator.IsAvailable)
                    EventFns.Forward(RootNavigatorLocator.GetPart(), new Suspending(SessionState));
                if (AuthenticationServiceLocator.IsAvailable)
                    EventFns.Forward(AuthenticationServiceLocator.GetPart(), new Suspending(SessionState));

                // Save the navigation state for all registered frames
                foreach (var weakFrameReference in RegisteredFrames)
                {
                    Frame frame;
                    if (weakFrameReference.TryGetTarget(out frame))
                    {
                        SaveFrameNavigationState(frame);
                    }
                }

                // Serialize the session state synchronously to avoid asynchronous access to shared
                // state
                var sessionData = new MemoryStream();
                var serializer = new DataContractSerializer(typeof(Dictionary<string, object>), KnownTypes);
                serializer.WriteObject(sessionData, _sessionState);

                // Get an output stream for the SessionState file and write the state asynchronously
                var file =
                    await
                    ApplicationData.Current.LocalFolder.CreateFileAsync(SessionStateFilename,
                                                                        CreationCollisionOption.ReplaceExisting);
                using (var fileStream = await file.OpenStreamForWriteAsync())
                {
                    sessionData.Seek(0, SeekOrigin.Begin);
                    await sessionData.CopyToAsync(fileStream);
                    await fileStream.FlushAsync();
                }
            }
            catch (Exception e)
            {
                throw new SuspensionManagerException(e);
            }
        }

        /// <summary>
        ///     Restores previously saved <see cref="SessionState" />.  Any <see cref="Frame" /> instances
        ///     registered with <see cref="RegisterFrame" /> will also restore their prior navigation
        ///     state, which in turn gives their active <see cref="Page" /> an opportunity restore its
        ///     state.
        /// </summary>
        /// <returns>
        ///     An asynchronous task that reflects when session state has been read.  The
        ///     content of <see cref="SessionState" /> should not be relied upon until this task
        ///     completes.
        /// </returns>
        public static async Task RestoreAsync()
        {
            _sessionState = new Dictionary<String, Object>();

            try
            {
                // Get the input stream for the SessionState file
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(SessionStateFilename);
                using (var inStream = await file.OpenSequentialReadAsync())
                {
                    // Deserialize the Session State
                    var serializer = new DataContractSerializer(typeof(Dictionary<string, object>), KnownTypes);
                    _sessionState = (Dictionary<string, object>) serializer.ReadObject(inStream.AsStreamForRead());
                }

                // Notify Root Navigator and AuthenticationService that we are restoring
                if (RootNavigatorLocator.IsAvailable)
                    EventFns.Forward(RootNavigatorLocator.GetPart(), new Restoring(SessionState));
                if (AuthenticationServiceLocator.IsAvailable)
                    EventFns.Forward(AuthenticationServiceLocator.GetPart(), new Restoring(SessionState));

                // Restore any registered frames to their saved state
                foreach (var weakFrameReference in RegisteredFrames)
                {
                    Frame frame;
                    if (weakFrameReference.TryGetTarget(out frame))
                    {
                        frame.ClearValue(FrameSessionStateProperty);
                        RestoreFrameNavigationState(frame);
                    }
                }
            }
            catch (Exception e)
            {
                throw new SuspensionManagerException(e);
            }
        }

        /// <summary>
        ///     Registers a <see cref="Frame" /> instance to allow its navigation history to be saved to
        ///     and restored from <see cref="SessionState" />.  Frames should be registered once
        ///     immediately after creation if they will participate in session state management.  Upon
        ///     registration if state has already been restored for the specified key
        ///     the navigation history will immediately be restored.  Subsequent invocations of
        ///     <see cref="RestoreAsync" /> will also restore navigation history.
        /// </summary>
        /// <param name="frame">
        ///     An instance whose navigation history should be managed by
        ///     <see cref="SuspensionManager" />
        /// </param>
        /// <param name="sessionStateKey">
        ///     A unique key into <see cref="SessionState" /> used to
        ///     store navigation-related information.
        /// </param>
        public static void RegisterFrame(Frame frame, String sessionStateKey)
        {
            if (frame.GetValue(FrameSessionStateKeyProperty) != null)
            {
                throw new InvalidOperationException("Frames can only be registered to one session state key");
            }

            if (frame.GetValue(FrameSessionStateProperty) != null)
            {
                throw new InvalidOperationException(
                    "Frames must be either be registered before accessing frame session state, or not registered at all");
            }

            // Use a dependency property to associate the session key with a frame, and keep a list of frames whose
            // navigation state should be managed
            frame.SetValue(FrameSessionStateKeyProperty, sessionStateKey);
            RegisteredFrames.Add(new WeakReference<Frame>(frame));

            // Check to see if navigation state can be restored
            RestoreFrameNavigationState(frame);
        }

        /// <summary>
        ///     Disassociates a <see cref="Frame" /> previously registered by <see cref="RegisterFrame" />
        ///     from <see cref="SessionState" />.  Any navigation state previously captured will be
        ///     removed.
        /// </summary>
        /// <param name="frame">
        ///     An instance whose navigation history should no longer be
        ///     managed.
        /// </param>
        public static void UnregisterFrame(Frame frame)
        {
            // Remove session state and remove the frame from the list of frames whose navigation
            // state will be saved (along with any weak references that are no longer reachable)
            SessionState.Remove((String) frame.GetValue(FrameSessionStateKeyProperty));
            RegisteredFrames.RemoveAll(weakFrameReference =>
                                            {
                                                Frame testFrame;
                                                return !weakFrameReference.TryGetTarget(out testFrame) ||
                                                       testFrame == frame;
                                            });
        }

        /// <summary>
        ///     Provides storage for session state associated with the specified <see cref="Frame" />.
        ///     Frames that have been previously registered with <see cref="RegisterFrame" /> have
        ///     their session state saved and restored automatically as a part of the global
        ///     <see cref="SessionState" />.  Frames that are not registered have transient state
        ///     that can still be useful when restoring pages that have been discarded from the
        ///     navigation cache.
        /// </summary>
        /// <param name="frame">The instance for which session state is desired.</param>
        /// <returns>
        ///     A collection of state subject to the same serialization mechanism as
        ///     <see cref="SessionState" />.
        /// </returns>
        public static Dictionary<String, Object> SessionStateForFrame(Frame frame)
        {
            var frameState = (Dictionary<String, Object>) frame.GetValue(FrameSessionStateProperty);

            if (frameState == null)
            {
                var frameSessionKey = (String) frame.GetValue(FrameSessionStateKeyProperty);
                if (frameSessionKey != null)
                {
                    // Registered frames reflect the corresponding session state
                    if (!_sessionState.ContainsKey(frameSessionKey))
                    {
                        _sessionState[frameSessionKey] = new Dictionary<String, Object>();
                    }
                    frameState = (Dictionary<String, Object>) _sessionState[frameSessionKey];
                }
                else
                {
                    // Frames that aren't registered have transient state
                    frameState = new Dictionary<String, Object>();
                }
                frame.SetValue(FrameSessionStateProperty, frameState);
            }
            return frameState;
        }

        private static void RestoreFrameNavigationState(Frame frame)
        {
            var frameState = SessionStateForFrame(frame);
            if (frameState.ContainsKey("Navigation"))
            {
                frame.SetNavigationState((String) frameState["Navigation"]);
            }
        }

        private static void SaveFrameNavigationState(Frame frame)
        {
            var frameState = SessionStateForFrame(frame);
            frameState["Navigation"] = frame.GetNavigationState();
        }
    }

    /// <summary>
    /// Exception thrown by the internal SuspensionManager for suspension failures.
    /// </summary>
    public class SuspensionManagerException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the SuspensionManagerException class.
        /// </summary>
        public SuspensionManagerException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the SuspensionManagerException class.
        /// </summary>
        /// <param name="e">Inner Exception</param>
        public SuspensionManagerException(Exception e) : base("SuspensionManager failed", e)
        {
        }
    }

    internal class Suspending
    {
        public Dictionary<string, object> SessionState { get; private set; }

        public Suspending(Dictionary<string, object> sessionState)
        {
            SessionState = sessionState;
        }
    }

    internal class Restoring
    {
        public Dictionary<string, object> SessionState { get; private set; }

        public Restoring(Dictionary<string, object> sessionState)
        {
            SessionState = sessionState;
        }
    }
}