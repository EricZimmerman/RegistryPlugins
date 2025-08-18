# RegistryPlugins

This repo that contains all the Registry Plugins used by Eric Zimmerman's Registry Explorer and [RECmd](https://github.com/EricZimmerman/RECmd). You can download both tools [here](https://ericzimmerman.github.io). 

## Ongoing Projects

 * [RegistryPlugins](https://github.com/EricZimmerman/RegistryPlugins/projects/1) - Development roadmap for Registry Explorer/RECmd Plugins. Please feel free to contribute by adding ideas or by finishing tasks in the `To Do` column. Any help is appreciated! 

## How Plugins Affect Output

Plugins are helpful in that they are able to display more data within the Registry in less rows. Using RECmd's CSV output, the `ValueData` column will be used to displayed parsed data regardless of whether a Plugin exists for a specific artifact. However, when a Plugin is being used to parse data from the Registry, the `ValueData2` and `ValueData3` are utilized to display more data relevant to the artifact while reducing the amount of rows within the CSV output. Additionally, Plugins are able to "translate" some of the data within the Registry, i.e., convert timestamps or convert Binary data into something human readable. 

## Plugin Output in Registry Explorer

This example highlights what the [UserAssist](https://github.com/EricZimmerman/RegistryPlugins/tree/master/RegistryPlugin.UserAssist) Plugin provides as an added benefit vs. the raw data values parsed from the Registry.

The `Values` tab shows the raw values within the Registry in their native ROT13 format. The `UserAssist` tab shows the ROT13 output converted into human readable data. 

![gif](https://github.com/rathbuna/RegistryPlugins/blob/master/RegistryExplorerPluginsInAction.gif)

## Plugin Output in RECmd (CSV)

You will know that a Plugin is being used within your RECmd CSV output if the `ValueType` columns displays `(plugin)` as the value. From there, the author of the Plugin will be able to organize the parsed data within the `ValueData`, `ValueData2`, and the `ValueData3` columns. This allows for more data to be parsed in less rows for more efficient and useful CSV output!

![test](https://github.com/rathbuna/RegistryPlugins/blob/master/RECmdPluginExampleOutput.jpg)

[RECmd Batch Files](https://github.com/EricZimmerman/RECmd/tree/master/BatchExamples) help make CSV output useful and efficient while reducing the noise. The [DFIR Batch File](https://github.com/EricZimmerman/RECmd/blob/master/BatchExamples/DFIRBatch.reb) is currently taking advantage of most, if not all, of the Registry Plugins within this repo. It is strongly recommended to use that Batch File when parsing the Windows Registry with RECmd. 
