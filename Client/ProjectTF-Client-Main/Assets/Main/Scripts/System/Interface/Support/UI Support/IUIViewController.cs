using System;

using BC.ODCC;

using UnityEngine;

namespace TFSystem
{
	public interface IUIViewController<TViewState> : IOdccObject where TViewState : Enum
	{
		public Awaitable OnInitViewState(TViewState viewState);
		public Awaitable OnChangeViewState(TViewState viewState);
		public void OnChangeViewState(TViewState viewState, Action<TViewState> callback);
	}
}
