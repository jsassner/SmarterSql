// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
namespace Sassner.SmarterSql.Utils.Menu {
	//	public abstract class MenuCommandHandler {
	//		private readonly CommandID _CommandId;
	//		private OleMenuCommand _MenuCommand;
	//
	//		protected internal MenuCommandHandler() {
	//			foreach (object attr in GetType().GetCustomAttributes(false)) {
	//				CommandIDAttribute idAttr = attr as CommandIDAttribute;
	//				if (idAttr != null) {
	//					_CommandId = new CommandID(idAttr.Guid, (int)idAttr.Command);
	//				}
	//			}
	//		}
	//
	//		public CommandID CommandId { get; }
	//		protected IServiceProvider ServiceProvider {
	//			get { return _Package; }
	//		}
	//
	//		protected OleMenuCommand MenuCommand { get; }
	//		public bool IsBound { get; }
	//
	//		protected virtual void OnExecute(OleMenuCommand command);
	//		protected virtual void OnQueryStatus(OleMenuCommand command);
	//		protected virtual void OnChange(OleMenuCommand command);
	//
	//		public void Bind() {
	//			if (_Package == null) {
	//				return;
	//			}
	//			OleMenuCommandService mcs = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
	//			if (mcs == null) {
	//				return;
	//			}
	//			_MenuCommand = new OleMenuCommand(ExecuteMenuCommandCallback, ChangeCallback, BeforeStatusQueryCallback, _CommandId);
	//			mcs.AddCommand(_MenuCommand);
	//		}
	//
	//		private void ExecuteMenuCommandCallback(object sender, EventArgs e) {
	//			OleMenuCommand command = sender as OleMenuCommand;
	//			if (command != null) {
	//				OnExecute(command);
	//			}
	//		}
	//
	//		private void ChangeCallback(object sender, EventArgs e) {
	//			OleMenuCommand command = sender as OleMenuCommand;
	//			if (command != null) {
	//				OnChange(command);
	//			}
	//		}
	//
	//		private void BeforeStatusQueryCallback(object sender, EventArgs e) {
	//			OleMenuCommand command = sender as OleMenuCommand;
	//			if (command != null) {
	//				OnQueryStatus(command);
	//			}
	//		}
	//	}
}