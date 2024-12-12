using System;

namespace TFContent
{
	[Serializable]
	public struct LinkInfo
	{
		public int linkIndex;
		public NodeDir linkDir;
		public enum NodeDir
		{
			X_Dir,
			Y_Dir,
			iX_Dir,
			iY_Dir
		}
	}
}
