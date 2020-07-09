# function_app
This repository contains code for Azure Function app which includes an Http Trigger function. The function can send github repository dispatch event when triggered. It is  modelled to send Azure events when subscribed to the event grid of the workspace with the endpoint as the function url. 

#### Basic Requirements to use the function:
1. Add personal access token in the application settings of the function app with the name **PAT_TOKEN**.
2. Add owner/repo name in the application settings of the function app with the name **REPO_NAME**.

### Events and its corresponding event types sent by the function:
```sh
Azure App Configuration Events
  1.Microsoft.AppConfiguration.KeyValueModified: appconfiguration-keyvaluemodified
  2.Microsoft.AppConfiguration.KeyValueDeleted: appconfiguration-keyvaluedeleted
  
Azure Machine Learning Events
  1.Microsoft.MachineLearningServices.ModelRegistered: machinelearning-modelregistered
  2.Microsoft.MachineLearningServices.ModelDeployed: machinelearning-modeldeployed
  3.Microsoft.MachineLearningServices.RunCompleted: machinelearning-runcompleted
  4.Microsoft.MachineLearningServices.DatasetDriftDetected: machinelearning-datadriftdetected
  5.Microsoft.MachineLearningServices.RunStatusChanged: machinelearning-runstatuschanged
```
  
### Example:
#### To trigger the workflow when the run is completed:
```sh

  On:
    repository_dispatch:
        types: [machinelearning-modelregistered]
  (...)

```
