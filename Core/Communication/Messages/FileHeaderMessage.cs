using System;
using System.Collections.Generic;
using Core.Communication.Packets;
using Core.Data.Account;
using Core.Data.File;

namespace Core.Communication.Messages
{
    [Serializable]
    public class FileHeaderMessage : SerializedPacket<FileHeaderMessage>, ICdnFile
    {
        public int FileParts { get; set; }
        
        public long Id { get; set; }
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        public double Version { get; set; }
        public AccessLevel AccessLevel { get; set; }
        public List<AccountReference> GrantedAccounts { get; set; } = new List<AccountReference>();

        public FileHeaderMessage() : base((int) PacketCodes.FileHeader)
        {
        }
        
    }
}