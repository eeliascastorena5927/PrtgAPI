﻿using System.Management.Automation;
using PrtgAPI.Parameters;

namespace PrtgAPI.PowerShell.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Creates a new set of device parameters for creating a brand new device under a group or probe.</para>
    /// 
    /// <para type="description">The New-DeviceParameters cmdlet creates a set of parameters for adding a brand
    /// new device to PRTG. Device parameter objects returned from New-DeviceParameters allow specifying
    /// a variety of device specific configuration details including Internet Protocol version and auto-discovery
    /// settings at the time of object creation.</para>
    /// 
    /// <para type="description">All devices must have a <see cref="Name"/> and <see cref="Host"/> property specified. If a <see cref="Host"/> (IP Address/HostName)
    /// is not specified in the call to New-DeviceParameters, PrtgAPI will automatically use the <see cref="Name"/> </para>
    /// as the hostname/IP Address.
    /// 
    /// <para type="description">Note that not all device parameters (such as settings that can be inherited from the parent group)
    /// can be specified with PrtgAPI at the time of object creation. If you wish to modify such properties, this can be achieved
    /// after the device has been created via the Set-ObjectProperty cmdlet.</para>
    /// 
    /// <example>
    ///     <code>C:\> $params = New-DeviceParameters dc-1</code>
    ///     <para>C:\> Get-Probe contoso | Add-Device $params</para>
    ///     <para>Create a new device named "dc-1" with hostname "dc-1" under the Contoso probe.</para>
    ///     <para/>
    /// </example>
    /// <example>
    ///     <code>C:\> $params = New-DeviceParameters exch-1 "2001:db8::ff00:42:8329"</code>
    ///     <para>C:\> $params.IPVersion = "IPv6"</para>
    ///     <para>C:\> Get-Probe contoso | Add-Device $params</para>
    ///     <para>Create a new device named "dc-2" with an IPv6 address, specifying the Internet Protocol as IPv6.</para>
    ///     <para/>
    /// </example>
    /// <example>
    ///     <code>C:\> $params = New-DeviceParameters dc-1</code>
    ///     <para>C:\> Get-Probe contoso | Add-Device $params</para>
    ///     <para>C:\> $device = Get-Probe contoso | Get-Device dc-1</para>
    ///     <para>C:\> $device | Set-ObjectProperty Location "23 Fleet Street"</para>
    ///     <para>Create a new device named "dc-1", retrieve the newly created device and then set its location.</para>
    /// </example>
    /// 
    /// <para type="link">Add-Device</para>
    /// <para type="link">Set-ObjectProperty</para>
    /// 
    /// </summary>
    [Cmdlet(VerbsCommon.New, "DeviceParameters")]
    public class NewDeviceParametersCommand : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The name to give the new device.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">The hostname or IP Address to use for the new device. If no value is specified, the name will be used.</para>
        /// </summary>
        [Parameter(Mandatory = false, Position = 1)]
        public new string Host { get; set; }

        /// <summary>
        /// Performs record-by-record processing functionality for the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            WriteObject(new NewDeviceParameters(Name, string.IsNullOrEmpty(Host) ? Name : Host));
        }
    }
}
