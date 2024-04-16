using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Ambulance
{
	internal class ResultLine
	{
		public static TextBlock GetLine(string col1, string col2)
		{
			Run run1 = new Run($"{col1}: ")
			{
				Foreground = DefaultProgramStyle.defaultColor,
			};
			Run run2 = new Run(col2);
	
			TextBlock textBlock = new TextBlock();
			textBlock.Inlines.Add(new Bold(run1));
			textBlock.Inlines.Add(run2);
			textBlock.TextWrapping = System.Windows.TextWrapping.WrapWithOverflow;
			textBlock.FontSize = 14;
			return textBlock;
		}
	}
}
