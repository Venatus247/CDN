using System;
using System.Collections.Generic;
using System.Text;
using Commons;
using Core.Exceptions;

//https://github.com/tom-weiland/tcp-udp-networking/blob/tutorial-part2/GameServer/GameServer/Packet.cs
//https://www.youtube.com/watch?v=4uHTSknGJaY

namespace Core.Communication.Packets
{
    /// <summary>Packet identifiers</summary>
    public enum PacketCodes
    {
        Ping = 1,
        FileHeader = 10,
        FilePart = 11,
        CdnConnected = 15,
        CdnSavedFile = 20
    }

    public class Packet : IDisposable
    {
        [NonSerialized]
        private readonly Encoding _stringEncoding = Encoding.UTF8;
        [NonSerialized]
        private List<byte> _buffer;
        [NonSerialized]
        private byte[] _readableBuffer;
        [NonSerialized]
        private int _readPosition;
        
        [NonSerialized]
        private bool _disposed;
        
        /// <summary>
        /// Creates a new empty packet (without an ID).
        /// </summary>
        public Packet()
        {
            _buffer = new List<byte>(); // Initialize buffer
            _readPosition = 0; // Set readPos to 0
        }

        /// <summary>
        /// Creates a new packet with a given ID. Used for sending.
        /// <param name="id">The packet ID.</param>
        ///</summary>
        public Packet(int id)
        {
            _buffer = new List<byte>();
            _readPosition = 0;

            Write(id);
        }

        /// <summary>
        /// Creates a packet from which data can be read. Used for receiving.
        /// <param name="data">The bytes to add to the packet.</param>
        /// </summary>
        public Packet(byte[] data)
        {
            _buffer = new List<byte>();
            _readPosition = 0;

            SetBytes(data);
        }

        public Packet(int id, byte[] data)
        {
            Logger.Debug($"Writing packet id {id}");
            _buffer = new List<byte>();
            _readPosition = 0;

            Write(id);
            SetBytes(data);
        }
        
        #region Functions
        /// <summary>
        /// Sets the packet's content and prepares it to be read.
        /// <param name="data">The bytes to add to the packet.</param>
        /// </summary>
        public void SetBytes(byte[] data)
        {
            Write(data);
            _readableBuffer = _buffer.ToArray();
        }

        /// <summary>
        /// Inserts the length of the packet's content at the start of the buffer.
        /// </summary>
        public void WriteLength()
        {
            _buffer.InsertRange(0, BitConverter.GetBytes(_buffer.Count));
        }

        /// <summary>
        /// Inserts the given int at the start of the buffer.
        /// <param name="value">The int to insert.</param>
        /// </summary>
        public void InsertInt(int value)
        {
            _buffer.InsertRange(0, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Gets the packet's content in array form.
        /// </summary>
        public byte[] ToArray()
        {
            _readableBuffer = _buffer.ToArray();
            return _readableBuffer;
        }

        /// <summary>
        /// Gets the length of the packet's content.
        /// </summary>
        public int Length()
        {
            return _buffer.Count;
        }

        /// <summary>
        /// Gets the length of the unread data contained in the packet.
        /// </summary>
        public int UnreadLength()
        {
            return Length() - _readPosition; // Return the remaining length (unread)
        }

        /// <summary>
        /// Resets the packet instance to allow it to be reused.
        /// <param name="shouldReset">Whether or not to reset the packet.</param>
        /// </summary>
        public void Reset(bool shouldReset = true)
        {
            if (shouldReset)
            {
                _buffer.Clear(); // Clear buffer
                _readableBuffer = null;
                _readPosition = 0; // Reset readPos
            }
            else
            {
                _readPosition -= 4; // "Unread" the last read int
            }
        }
        #endregion

        #region Write Data
        /// <summary>
        /// Adds a byte to the packet.
        /// <param name="value">The byte to add.</param>
        /// </summary>
        public void Write(byte value)
        {
            _buffer.Add(value);
        }
        /// <summary>
        /// Adds an array of bytes to the packet.
        /// <param name="value">The byte array to add.</param>
        /// </summary>
        public void Write(byte[] value)
        {
            _buffer.AddRange(value);
        }
        /// <summary>
        /// Adds a short to the packet.
        /// <param name="value">The short to add.</param>
        /// </summary>
        public void Write(short value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
        }
        /// <summary>
        /// Adds an int to the packet.
        /// <param name="value">The int to add.</param>
        /// </summary>
        public void Write(int value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
        }
        /// <summary>
        /// Adds a long to the packet.
        /// <param name="value">The long to add.</param>
        /// </summary>
        public void Write(long value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
        }
        /// <summary>
        /// Adds a float to the packet.
        /// <param name="value">The float to add.</param>
        /// </summary>
        public void Write(float value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
        }
        /// <summary>
        /// Adds a bool to the packet.
        /// <param name="value">The bool to add.</param>
        /// </summary>
        public void Write(bool value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
        }
        /// <summary>
        /// Adds a string to the packet.
        /// <param name="value">The string to add.</param>
        /// </summary>
        public void Write(string value)
        {
            Write(value.Length); // Add the length of the string to the packet
            _buffer.AddRange(_stringEncoding.GetBytes(value)); // Add the string itself
        }
        #endregion

        #region Read Data
        /// <summary>
        /// Reads a byte from the packet.
        /// <param name="moveReadPosition">Whether or not to move the buffer's read position.</param>
        /// </summary>
        public byte ReadByte(bool moveReadPosition = true)
        {
            //check for unread bytes
            if (_buffer.Count <= _readPosition) 
                throw new BufferIndexOutOfRangeException("Could not read value of type 'byte'!");
            
            var value = _readableBuffer[_readPosition]; // Get the byte at readPos' position
            if (moveReadPosition)
            {
                // If _moveReadPos is true
                _readPosition += 1; // Increase readPos by 1
            }
            return value; // Return the byte
        }

        /// <summary>
        /// Reads an array of bytes from the packet.
        /// <param name="length">The length of the byte array.</param>
        /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
        /// </summary>
        public byte[] ReadBytes(int length, bool moveReadPos = true)
        {
            //check for unread bytes
            if (_buffer.Count <= _readPosition) 
                throw new BufferIndexOutOfRangeException("Could not read value of type 'byte[]'!");
            
            var value = _buffer.GetRange(_readPosition, length).ToArray(); // Get the bytes at readPos' position with a range of _length
            if (moveReadPos)
            {
                _readPosition += length;
            }
            return value;
        }

        /// <summary>
        /// Reads a short from the packet.
        /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
        /// </summary>
        public short ReadShort(bool moveReadPos = true)
        {
            //check for unread bytes
            if (_buffer.Count <= _readPosition) 
                throw new BufferIndexOutOfRangeException("Could not read value of type 'short'!");
            
            var value = BitConverter.ToInt16(_readableBuffer, _readPosition);
            if (moveReadPos)
            {
                _readPosition += 2;
            }
            return value;
        }

        /// <summary>
        /// Reads an int from the packet.
        /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
        /// </summary>
        public int ReadInt(bool moveReadPos = true)
        {
            //check for unread bytes
            if (_buffer.Count <= _readPosition)
                throw new BufferIndexOutOfRangeException("Could not read value of type 'int'!");
            
            var value = BitConverter.ToInt32(_readableBuffer, _readPosition);
            if (moveReadPos)
            {
                _readPosition += 4;
            }
            return value;
        }

        /// <summary>
        /// Reads a long from the packet.
        /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
        /// </summary>
        public long ReadLong(bool moveReadPos = true)
        {
            if (_buffer.Count <= _readPosition)
                throw new BufferIndexOutOfRangeException("Could not read value of type 'long'!");
            
            var value = BitConverter.ToInt64(_readableBuffer, _readPosition); // Convert the bytes to a long
            if (moveReadPos)
            {
                _readPosition += 8;
            }
            return value;
        }

        /// <summary>
        /// Reads a float from the packet.
        /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
        /// </summary>
        public float ReadFloat(bool moveReadPos = true)
        {
            //check for unread bytes
            if (_buffer.Count <= _readPosition) 
                throw new BufferIndexOutOfRangeException("Could not read value of type 'float'!");
            
            var value = BitConverter.ToSingle(_readableBuffer, _readPosition);
            if (moveReadPos)
            {
                _readPosition += 4;
            }
            return value;
        }

        /// <summary>
        /// Reads a bool from the packet.
        /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
        /// </summary>
        public bool ReadBool(bool moveReadPos = true)
        {
            //check for unread bytes
            if (_buffer.Count <= _readPosition)
                throw new BufferIndexOutOfRangeException("Could not read value of type 'bool'!");
            
            var value = BitConverter.ToBoolean(_readableBuffer, _readPosition);
            if (moveReadPos)
            {
                _readPosition += 1;
            }
            return value;
        }
        
        /// <summary>
        /// Reads a string from the packet.
        /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
        /// </summary>
        public string ReadString(bool moveReadPos = true)
        {
            try
            {
                var length = ReadInt();
                var value = _stringEncoding.GetString(_readableBuffer, _readPosition, length);
                if (moveReadPos && value.Length > 0)
                {
                    _readPosition += length;
                }
                return value;
            }
            catch
            {
                throw new Exception("Could not read value of type 'string'!");
            }
        }
        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            
            if (disposing)
            {
                _buffer = null;
                _readableBuffer = null;
                _readPosition = 0;
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}