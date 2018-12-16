using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

public class UnityConsoleRedirect : TextWriter {

    public override Encoding Encoding { get { return Encoding.Default; } }

    private StringBuilder buffer = new StringBuilder();

    public static void Redirect()
    {
        Console.SetOut(new UnityConsoleRedirect());
    }

    public override void Flush()
    {
        Debug.Log(buffer.ToString());
        buffer.Length = 0;
    }

    public override void Write(bool value)
    {
        buffer.Append(value);
    }

    public override void Write(char value)
    {
        buffer.Append(value);
    }

    //public override void Write(char[] buffer)
    //{
    //    buffer.Append(buffer);
    //}

    public override void Write(decimal value)
    {
        buffer.Append(value);
    }

    public override void Write(double value)
    {
        buffer.Append(value);
    }

    public override void Write(int value)
    {
        buffer.Append(value);
    }

    public override void Write(long value)
    {
        buffer.Append(value);
    }

    public override void Write(object value)
    {
        buffer.Append(value);
    }

    public override void Write(float value)
    {
        buffer.Append(value);
    }

    public override void Write(string value)
    {
        buffer.Append(value);
    }

    public override void Write(uint value)
    {
        buffer.Append(value);
    }

    public override void Write(ulong value)
    {
        buffer.Append(value);
    }

    public override void Write(string format, object arg0)
    {
        buffer.AppendFormat(format, arg0);
    }

    public override void Write(string format, params object[] arg)
    {
        buffer.AppendFormat(format, arg);
    }

    public override void Write(char[] buffer, int index, int count)
    {
        Write(new string(buffer, index, count));
    }

    public override void Write(string format, object arg0, object arg1)
    {
        buffer.AppendFormat(format, arg0, arg1);
    }

    public override void Write(string format, object arg0, object arg1, object arg2)
    {
        buffer.AppendFormat(format, arg0, arg1, arg2);
    }

    //public override void WriteLine()
    //{
    //    buffer.AppendLine();
    //}

    //public override void WriteLine(bool value)
    //{
    //    base.WriteLine(value);
    //}

    //public override void WriteLine(char value)
    //{
    //    base.WriteLine(value);
    //}

    //public override void WriteLine(char[] buffer)
    //{
    //    base.WriteLine(buffer);
    //}

    //public override void WriteLine(decimal value)
    //{
    //    base.WriteLine(value);
    //}

    //public override void WriteLine(double value)
    //{
    //    base.WriteLine(value);
    //}

    //public override void WriteLine(int value)
    //{
    //    base.WriteLine(value);
    //}

    //public override void WriteLine(long value)
    //{
    //    base.WriteLine(value);
    //}

    //public override void WriteLine(object value)
    //{
    //    base.WriteLine(value);
    //}

    //public override void WriteLine(float value)
    //{
    //    base.WriteLine(value);
    //}

    //public override void WriteLine(string value)
    //{
    //    base.WriteLine(value);
    //}

    //public override void WriteLine(uint value)
    //{
    //    base.WriteLine(value);
    //}

    //public override void WriteLine(ulong value)
    //{
    //    base.WriteLine(value);
    //}

    //public override void WriteLine(string format, object arg0)
    //{
    //    base.WriteLine(format, arg0);
    //}

    //public override void WriteLine(string format, params object[] arg)
    //{
    //    base.WriteLine(format, arg);
    //}

    //public override void WriteLine(char[] buffer, int index, int count)
    //{
    //    base.WriteLine(buffer, index, count);
    //}

    //public override void WriteLine(string format, object arg0, object arg1)
    //{
    //    base.WriteLine(format, arg0, arg1);
    //}

    //public override void WriteLine(string format, object arg0, object arg1, object arg2)
    //{
    //    base.WriteLine(format, arg0, arg1, arg2);
    //}
}
