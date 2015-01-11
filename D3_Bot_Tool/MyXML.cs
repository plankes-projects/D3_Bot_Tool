using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D3_Bot_Tool
{
    class MyXML
    {
        private String file_name_;
        private String file_value_;

        public MyXML(String file_name = "", String file_value = "")
        {
            file_name_ = file_name;

            if (file_value == "")
            {
                if (System.IO.File.Exists(file_name_))
                    file_value_ = System.IO.File.ReadAllText(file_name_);
                else
                    file_value_ = "";
            }
            else
                file_value_ = file_value;
        }

        public String getFileValue()
        {
            return file_value_;
        }

        public void setFileName(String file_name)
        {
            file_name_ = file_name;
            if (System.IO.File.Exists(file_name_))
                file_value_ = System.IO.File.ReadAllText(file_name_);
            else
                file_value_ = "";
        }

        private String getStartKey(String key)
        {
            return "<" + key + ">\n";
        }

        private String getEndKey(String key)
        {
            return "\n</" + key + ">\n";
        }

        public void write(String key, String value)
        {
            String begin_key = getStartKey(key);
            String end_key = getEndKey(key);

            int start = file_value_.IndexOf(begin_key);
            int end = file_value_.IndexOf(end_key);
            if (start == -1 || end == -1)
                file_value_ += begin_key + value + end_key;
            else
                file_value_ = file_value_.Remove(start, end - start + end_key.Length).Insert(start, begin_key + value + end_key);

            System.IO.File.WriteAllText(file_name_, file_value_);
        }

        public String read(String key)
        {
            String begin_key = getStartKey(key);
            String end_key = getEndKey(key);

            int start = file_value_.IndexOf(begin_key);
            int end = file_value_.IndexOf(end_key);
            if (start == -1 || end == -1)
                return "";
            return file_value_.Substring(start + begin_key.Length, end - start - begin_key.Length);
        }
    }
}
