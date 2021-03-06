﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LoveCoody.handle
{
    class StreamHandle
    {

        public static String loadSockerStream(Stream stream) {
            StreamReader streamReader = new StreamReader(stream);
            StringBuilder resultContext = new StringBuilder();
            String line= streamReader.ReadLine();
            while (!String.IsNullOrEmpty(line)) {
                resultContext.AppendLine(line);
                line = streamReader.ReadLine();
            }
            return null;
        }
    }
}
