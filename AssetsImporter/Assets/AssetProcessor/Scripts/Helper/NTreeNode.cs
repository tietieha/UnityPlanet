// **********************************************************
// *		                .-"""-.							*
// *		               / .===. \			            *
// *		               \/ 6 6 \/			            *
// *		     ______ooo__\__=__/_____________			*
// *		    / @author     Leon			   /			*
// *		   / @Modified   2024-07-04       /			    *
// *		  /_____________________ooo______/			    *
// *		  			    |_ | _|			                *
// *		  			    /-'Y'-\			                *
// *		  			   (__/ \__)			            *
// **********************************************************

using System.Collections.Generic;

namespace AssetProcessor
{
	public class NTreeNode<T>
	{
		public T Data { get; set; }
		
		public NTreeNode<T> Parent { get; set; }
		public List<NTreeNode<T>> Children { get; set; }

		public NTreeNode(T data)
		{
			Data = data;
			Children = new List<NTreeNode<T>>();
		}

		public void AddChild(NTreeNode<T> child)
		{
			Children.Add(child);
		}

		public NTreeNode<T> GetChild(T data)
		{
			return Children.Find(n => n.Data.Equals(data));
		}
	}
}