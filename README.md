# RegistryPlugins

This repo that contains all the Registry Plugins used by Eric Zimmerman's Registry Explorer and [RECmd](https://github.com/EricZimmerman/RECmd). You can download both tools [here](https://f001.backblazeb2.com/file/EricZimmermanTools/RegistryExplorer_RECmd.zip). 

## How Plugins Affect Output

Plugins are helpful in that they are able to display more data within the Registry in less rows. Using RECmd's CSV output, the `ValueData` column will be used to displayed parsed data regardless of whether a Plugin exists for a specific artifact. However, when a Plugin is being used to parse data from the Registry, the `ValueData2` and `ValueData3` are utilized to display more data relevant to the artifact while reducing the amount of rows within the CSV output. Additionally, Plugins are able to "translate" some of the data within the Registry, i.e., convert timestamps or convert Binary data into something human readable. 

## Plugin Output in Registry Explorer

This example highlights what the UserAssist Plugin provides as an added benefit vs. the raw data values parsed from the Registry.

Values tab shows the raw values within the Registry in their native ROT13 format. The UserAssist tab shows the ROT13 output converted into human readable data. 

![gif](https://github.com/rathbuna/RegistryPlugins/blob/master/RegistryExplorerPluginsInAction.gif)

## Plugin Output in RECmd (CSV)

You will know that a Plugin is being used within your RECmd CSV output if the `ValueType` columns displays `(plugin)` as the value. From there, the author of the Plugin will be able to organize the parsed data within the `ValueData`, ValueData2`, and the `ValueData3` columns. This allows for more data to be parsed in less rows for more efficient and useful CSV output!

![test](https://github.com/rathbuna/RegistryPlugins/blob/master/RECmdPluginExampleOutput.jpg)
