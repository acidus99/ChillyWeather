namespace Chilly;

using System.IO;
using Chilly.Models;

public abstract class AbstractForcastRenderer
{
    protected TextWriter _fout;
    protected Formatter _formatter;

    public AbstractForcastRenderer(TextWriter fout)
    {
        _fout = fout;
        _formatter = new Formatter();
    }

    public abstract void Render(Forecast forecast);
}

