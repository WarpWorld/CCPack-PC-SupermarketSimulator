using System.Net.Sockets;
using System.Text;

namespace CrowdControl;

/// <summary>Reads data from a socket until a null terminator is encountered.</summary>
/// <remarks>
/// Many network stream parser implementations either don't properly handle multibyte UTF-8 characters or perform wasteful microallocations.
/// This class avoids both problems and is recommended for use in production code when possible.
/// </remarks>
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class DelimitedStreamReader : IDisposable
{
    private readonly MemoryStream _memory_stream = new();
    private readonly NetworkStream m_stream;

    /// <summary>Reads data from a socket until a null terminator is encountered.</summary>
    /// <param name="stream">The network stream to read from.</param>
    /// <remarks>
    /// Many network stream parser implementations either don't properly handle multibyte UTF-8 characters or perform wasteful microallocations.
    /// This class avoids both problems and is recommended for use in production code when possible.
    /// </remarks>
    public DelimitedStreamReader(NetworkStream stream)
    {
        m_stream = stream;
    }

    ~DelimitedStreamReader() => Dispose(false);

    /// <summary>Disposes the internal memory buffer.</summary>
    public void Dispose() => Dispose(true);

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;
        try { _memory_stream.Dispose(); }
        catch { /**/ }
    }

    /// <summary>Reads data from the socket until a null terminator is encountered.</summary>
    /// <returns>The data read from the socket.</returns>
    public string ReadUntilNullTerminator()
    {
        int byteRead;

        while ((byteRead = m_stream.ReadByte()) != -1)
        {
            if (byteRead == 0x00) // null terminator
                break;

            _memory_stream.WriteByte((byte)byteRead);
        }

        if (byteRead == -1)
            throw new EndOfStreamException("Reached end of stream without finding a null terminator.");

        string result = Encoding.UTF8.GetString(_memory_stream.ToArray());
        _memory_stream.SetLength(0);
        return result;
    }
}
