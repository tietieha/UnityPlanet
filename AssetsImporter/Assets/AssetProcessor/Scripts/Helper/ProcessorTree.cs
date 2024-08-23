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

using System.Collections;
using System.Collections.Generic;

namespace AssetProcessor
{
	public class ProcessorTree : IEnumerable
	{
		private readonly NTreeNode<AssetProcessorConfigNode> root;
		public NTreeNode<AssetProcessorConfigNode> Root => this.root;
		public List<NTreeNode<AssetProcessorConfigNode>> Nodes => this.root.Children;
		
		
		
		public IEnumerator GetEnumerator()
		{
			return Nodes.GetEnumerator();
		}
	}
}