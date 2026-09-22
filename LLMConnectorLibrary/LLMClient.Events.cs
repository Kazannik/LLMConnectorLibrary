// Ignore Spelling: Uri

using LLMConnectorLibrary.EventArgs;
using System;

namespace LLMConnectorLibrary
{
	partial class LLMClient
	{
		#region Events

		public event EventHandler<ClientErrorEventArgs> ClientError;
		protected virtual void OnClientError(ClientErrorEventArgs e) => ClientError?.Invoke(this, e);

		public event EventHandler<CheckHostEventArgs> HostChecked;
		protected virtual void OnHostChecked(CheckHostEventArgs e) => HostChecked?.Invoke(this, e);

		public event EventHandler<ModelsCollectionCompletedEventArgs> ModelsCollectionCompleted;
		protected virtual void OnModelsCollectionCompleted(ModelsCollectionCompletedEventArgs e) =>
			ModelsCollectionCompleted?.Invoke(this, e);

		public event EventHandler<ChatCompletedEventArgs> ChatCompleted;
		public event EventHandler<ChatCanceledEventArgs> ChatCanceled;
		public event EventHandler<ChatProgressEventArgs> ChatProgress;

		protected virtual void OnChatCompleted(ChatCompletedEventArgs e) => ChatCompleted?.Invoke(this, e);
		protected virtual void OnChatCanceled(ChatCanceledEventArgs e) => ChatCanceled?.Invoke(this, e);
		protected virtual void OnChatProgress(ChatProgressEventArgs e) => ChatProgress?.Invoke(this, e);

		public event EventHandler<EmbedCompletedEventArgs> EmbedCompleted;
		public event EventHandler<EmbedCanceledEventArgs> EmbedCanceled;
		public event EventHandler<EmbedProgressEventArgs> EmbedProgress;

		protected virtual void OnEmbedCompleted(EmbedCompletedEventArgs e) => EmbedCompleted?.Invoke(this, e);
		protected virtual void OnEmbedCanceled(EmbedCanceledEventArgs e) => EmbedCanceled?.Invoke(this, e);
		protected virtual void OnEmbedProgress(EmbedProgressEventArgs e) => EmbedProgress?.Invoke(this, e);

		#endregion

	}
}
