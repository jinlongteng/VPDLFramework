using System;
using System.Collections.Generic;
using System.Text;
using VPDLFrameworkLib;


public class ECFfpScriptProcessor : IECFfpScriptProcessor
{
    /// <summary>
    /// This class is used to communicate with PLC, when set user data enable,ProcessInputData will be executed. You can use some script
    /// process it and create the InputChanged event. All results from streams or groups will enter the ProcessOutputData,you can use some
    /// script process it and create the SendToPLC event.
    /// </summary>
    public ECFfpScriptProcessor()
    {

    }


    /// <summary>
    /// Print a log on UI
    /// </summary>
    public event EventHandler<string> PrintLog;

    /// <summary>
    /// Input changed,convert PLC data to Native Mode Command
    /// </summary>
    public event EventHandler<string> InputChanged;

    /// <summary>
    /// Send PLC some data
    /// </summary>
    public event EventHandler<KeyValuePair<int, byte[]>> SendToPLC;

    /// <summary>
    /// When event SetUserData occurces, user data will be processed
    /// </summary>
    /// <param name="data">User data</param>
    /// <returns>Native Mode Command</returns>
    public void ProcessInputData(byte[] data)
    {

    }

    /// <summary>
    /// Both the results of groups and the work streams will be passed 
    /// into the method, parameter format "Group/Stream name,ResultForSend",
    /// and the ResultForSend comes from ToolBlock's output terminal
    /// </summary>
    /// <param name="data">"Group/Stream name,ResultForSend",ResultForSend from ToolBlock outputs["ResultForSend"]</param>
    /// <returns>Returns the data address area offset and the result data</returns>
    public void ProcessOutputData(string data)
    {


    }



    /// <summary>
    /// This method will be called when the object is destroyed
    /// </summary>
    public void Dispose()
    {

    }
}
