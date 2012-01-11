using IdeaBlade.Core.Composition;

namespace Common.Messages
{
    [InterfaceExport(typeof (IMessageProcessor))]
    public interface IMessageProcessor
    {
    }
}