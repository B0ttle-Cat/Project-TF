using System;

using BC.ODCC;

using UnityEngine;

namespace TF.System
{

	public interface IUIViewController : IOdccComponent
	{

	}
	public interface IUIViewController<TViewState> : IUIViewController where TViewState : Enum
	{
		public void OnInitViewState(TViewState viewState);

		public Awaitable OnChangeViewState(TViewState viewState);
		public void OnChangeViewState(TViewState viewState, Action<TViewState> callback);
	}
}
