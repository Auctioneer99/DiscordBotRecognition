using Discord.Audio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBotRecognitionCore.Synthesier
{
    /*
    public class DiscordVoiceOutputStream : Stream
    {
        const int BUFFER_LENGTH = 3840;

        private AudioInStream _stream;

        public DiscordVoiceOutputStream(AudioInStream stream)
        {
            _stream = stream;
        }

        public override bool CanRead => _stream.CanRead;

        public override bool CanSeek => _stream.CanSeek;

        public override bool CanWrite => _stream.CanWrite;

        public override long Length => _stream.Length;

        public override long Position { get => _stream.Position; set => _stream.Position = value; }

        public override void Flush()
        {
            _stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }
        
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (buffer.Length != BUFFER_LENGTH || count != BUFFER_LENGTH)
            {
                throw new ArgumentException($"Count and length must be equal to {BUFFER_LENGTH}");
            }

            var frame = await _stream.ReadFrameAsync(cancellationToken);
            frame.Payload.CopyTo(buffer, 0);
            return frame.Payload.Length;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }
    }*/
}
