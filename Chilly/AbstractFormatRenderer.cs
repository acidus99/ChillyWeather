using System;
using System.IO;
using Chilly.Models;

namespace Chilly
{
	public abstract class AbstractFormatRenderer
	{
        protected TextWriter _fout;
        protected Formatter _formatter;

        public AbstractFormatRenderer(TextWriter fout)
        {
            _fout = fout;
            _formatter = new Formatter();
        }

        public abstract void Render(Forecast forecast);
	}
}

