﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomotiveNVHSuite
{
    public class CommandAndPayload
    {
        public string Command;
        public string Payload;
    }

    public class PropertyPair
    {
        public string PropertyName { get; set; }
        public string PropertyValue { get; set; }
    }

    public class FileInformation
    {
        public int NumSequences;
        public string[] SequenceNames;
    }

    public class FileContents
    {
        
    }
    //public class Handle
}
