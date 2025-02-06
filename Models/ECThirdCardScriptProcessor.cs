using System;
using System.Collections.Generic;
using System.Text;
using VPDLFrameworkLib;

public class ECThirdCardScriptProcessor : IECThirdCardScriptProcessor
{
    /// <summary>
    /// There are two ways to execute Native Mode Command in this script, 
    /// one is to enable the timer to return Native Mode Command by continuously
    /// scanning the input state in the method ReadInputStatus, and the other is to generate 
    /// event containing Native Mode Command through any custom method
    /// </summary>
    public ECThirdCardScriptProcessor()
    {
        IsEnableTimer = true;
        
    }

    /// <summary>
    /// If the timer is enabled, the ReadInputStatus method will be executed in each timer event
    /// </summary>
    private bool _isEnableTimer;

    public bool IsEnableTimer
    {
        get{ return this._isEnableTimer; }
        set { this._isEnableTimer = value;}
    }

    /// <summary>
    /// Input changed event, parameter should be Native Mode Command.You can generate this event 
    /// when an input signal is detected by any method you define
    /// </summary>
    public event EventHandler<string> InputChanged;

    /// <summary>
    /// Send a message by specified tcp device
    /// </summary>
    public event EventHandler<KeyValuePair<string, string>> SendTCPMessage;

    /// <summary>
    /// Print a log on UI
    /// </summary>
    public event EventHandler<string> PrintLog;

    /// <summary>
    /// Read input status, return Native Mode Command
    /// Description:
    /// The method runs continuously at 2ms intervals, write code 
    /// that can read the input state and return the Native Mode Command 
    /// that need to be executed when a signal is detected
    /// </summary>
    /// <returns>Native Mode Command</returns>
    public string ReadInputStatus()
    {


        return "";
    }

    /// <summary>
    /// Write Output Status
    /// Description:
    /// Both the results of groups and the work streams will be passed 
    /// into the method, parameter format "Group/Stream name,ResultForSend",
    /// and the ResultForSend comes from ToolBlock's output terminal
    /// </summary>
    /// <param name="result">"Group/Stream name,ResultForSend",ResultForSend from ToolBlock outputs["ResultForSend"]</param>
    public void WriteOutputStatus(string result)
    {

    }

    /// <summary>
    /// This method will be called when the object is destroyed
    /// </summary>
    public void Dispose()
    {
        
    }

    /// <summary>
    /// When other board are enabled in the setup window, each message form TCP/IP's devices will be passed into this method execution
    /// </summary>
    /// <param name="tcpName">tcp name</param>
    /// <param name="msg">message</param>
    public void OnTCPMessageCome(string tcpName, string msg)
    {
        
    }

    /// <summary>
    /// Method will be execute when Image Source acquire image completed
    /// </summary>
    /// <param name="imageSourceName">Image Source name</param>
    /// <param name="isCameraMode">Is camera mode or not</param>
    public void OnImageSourceCompelted(string imageSourceName, bool isCameraMode)
    {
        
    }
}

