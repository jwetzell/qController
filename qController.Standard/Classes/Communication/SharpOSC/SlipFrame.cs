using System.Collections.Generic;

namespace SharpOSC
{
    public static class SlipFrame
    {
        public static readonly byte END = 0xc0;
        public static readonly byte ESC = 0xdb;
        public static readonly byte ESC_END = 0xDC;
        public static readonly byte ESC_ESC = 0xDD;

        public static List<byte[]> Decode(byte[] data)
        {
            List<byte[]> messages = new List<byte[]>();

            List<byte> buffer = new List<byte>();
            bool escapeNext = false;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == ESC)
                {
                    escapeNext = true;
                    continue;
                }

                if (escapeNext)
                {
                    if (data[i] == ESC_END)
                    {
                        buffer.Add(END);
                    }
                    else if (data[i] == ESC_ESC)
                    {
                        buffer.Add(ESC);
                    }
                }
                else if (data[i] == END)
                {
                    messages.Add(buffer.ToArray());
                    buffer.Clear();
                }
                else
                {
                    buffer.Add(data[i]);
                }
            }

            return messages;
        }

        public static byte[] Encode(byte[] data)
        {
            List<byte> slipData = new List<byte>();

            byte[] esc_end = { ESC, ESC_END };
            byte[] esc_esc = { ESC, ESC_ESC };
            byte[] end = { END };

            int length = data.Length;
            for (int i = 0; i < length; i++)
            {
                if (data[i] == END)
                {
                    slipData.AddRange(esc_end);
                }
                else if (data[i] == ESC)
                {
                    slipData.AddRange(esc_esc);
                }
                else
                {
                    slipData.Add(data[i]);
                }
            }
            slipData.AddRange(end);
            return slipData.ToArray();
        }
    }
}