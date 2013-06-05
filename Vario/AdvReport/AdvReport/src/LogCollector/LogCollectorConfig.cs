/* GPL > 3.0
Copyright (C) 1996-2008 eIrOcA Enrico Croce & Simona Burzio

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;   
using System.Configuration;

namespace Reporting {

  /// <summary>
  /// Configuration section &lt;Class1&gt;
  /// </summary>
  /// <remarks>
  /// Assign properties to your child class that has the attribute 
  /// <c>[ConfigurationProperty]</c> to store said properties in the xml.
  /// </remarks>
  public sealed class LogCollectorConfig : ConfigurationSection {

    Configuration _Config;

    #region ConfigurationProperties
    

    /// <summary>
    /// Collection of <c>Class1Element(s)</c> 
    /// A custom XML section for an applications configuration file.
    /// </summary>
    [ConfigurationProperty("exampleAttribute", DefaultValue="exampleValue")]
    public string ExampleAttribute {
      get { return (string) this["exampleAttribute"]; }
      set { this["exampleAttribute"] = value; }
    }


    #endregion

    /// <summary>
    /// Private Constructor used by our factory method.
    /// </summary>
    private LogCollectorConfig () : base () {
      // Allow this section to be stored in user.app. By default this is forbidden.
      this.SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
    }

    #region Public Methods
    
    /// <summary>
    /// Saves the configuration to the config file.
    /// </summary>
    public void Save() {
      _Config.Save();
    }
    
    #endregion
    
    #region Static Members
    
    /// <summary>
    /// Gets the current applications &lt;Class1&gt; section.
    /// </summary>
    /// <param name="ConfigLevel">
    /// The &lt;ConfigurationUserLevel&gt; that the config file
    /// is retrieved from.
    /// </param>
    /// <returns>
    /// The configuration file's &lt;Class1&gt; section.
    /// </returns>
    public static LogCollectorConfig GetSection (ConfigurationUserLevel ConfigLevel) {
      Configuration Config = ConfigurationManager.OpenExeConfiguration(ConfigLevel);
      LogCollectorConfig settings;
      settings = (LogCollectorConfig)Config.GetSection("LogCollector");
      if (settings == null) {
        settings = new LogCollectorConfig();
        Config.Sections.Add("LogCollector", settings);
      }
      settings._Config = Config;
      return settings;
    }
    
    #endregion
  }
}

