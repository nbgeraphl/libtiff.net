﻿using System;
using System.Collections.Generic;
using System.Text;

using BitMiracle.LibJpeg.Classic;

namespace BitMiracle.LibTiff.Internal
{
    /// <summary>
    /// Alternate destination manager for outputting to JPEGTables field.
    /// </summary>
    class JpegTablesDestination : jpeg_destination_mgr
    {
        private JpegCodec m_sp;

        public JpegTablesDestination(JpegCodec sp)
        {
            m_sp = sp;
        }

        public override void init_destination()
        {
            /* while building, jpegtables_length is allocated buffer size */
            initInternalBuffer((JOCTET*)m_sp->m_jpegtables, m_sp->m_jpegtables_length);
        }

        public override bool empty_output_buffer()
        {
            /* the entire buffer has been filled; enlarge it by 1000 bytes */
            byte* newbuf = Tiff::Realloc(m_sp->m_jpegtables, m_sp->m_jpegtables_length, (int)(m_sp->m_jpegtables_length + 1000));
            if (newbuf == NULL)
                m_sp->m_compression->ERREXIT1(JERR_OUT_OF_MEMORY, 100);

            initInternalBuffer((JOCTET*)newbuf + m_sp->m_jpegtables_length, 1000);
            m_sp->m_jpegtables = newbuf;
            m_sp->m_jpegtables_length += 1000;
            return true;
        }

        public override void term_destination()
        {
            /* set tables length to number of bytes actually emitted */
            m_sp->m_jpegtables_length -= freeInBuffer();
        }
    }
}