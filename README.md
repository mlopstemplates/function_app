 [![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fgithub.com%2FAyaz43%2Ffunction_app%2Fblob%2Fmaster%2F.cloud%2F.azure%2Fazuredeploy.json)

# function_app 
This repository contains code for Azure Function app which includes an Http Trigger function. The function can send github repository dispatch event when triggered. It is  modelled to send Azure events when subscribed to the event grid of the workspace with the endpoint as the function url. 

#### Basic Requirements to use the function:
1. Add personal access token in the application settings of the function app with the name **PAT_TOKEN**.

### Events and its corresponding event types sent by the function: 
```sh
Azure App Configuration Events
``` [Link](https://docs.microsoft.com/en-us/azure/event-grid/event-schema-app-configuration/)
```sh
  1.Microsoft.AppConfiguration.KeyValueModified: appconfiguration-keyvaluemodified
  2.Microsoft.AppConfiguration.KeyValueDeleted: appconfiguration-keyvaluedeleted
  
Azure App Service Events
  1.Microsoft.Web/sites.BackupOperationStarted: web/sites-backupoperationstarted
  2.Microsoft.Web/sites.BackupOperationCompleted: web/sites-backupoperationcompleted	
  3.Microsoft.Web/sites.BackupOperationFailed: web/sites-backupoperationfailed	
  4.Microsoft.Web/sites.RestoreOperationStarted: web/sites-restoreoperationstarted
  5.Microsoft.Web/sites.RestoreOperationCompleted: web/sites-restoreoperationcompleted
  6.Microsoft.Web/sites.RestoreOperationFailed: web/sites-restoreoperationfailed
  7.Microsoft.Web/sites.SlotSwapStarted: web/sites-slowswapstarted
  8.Microsoft.Web/sites.SlotSwapCompleted: web/sites-slowswapcompleted
  10.Microsoft.Web/sites.SlotSwapFailed: web/sites-slowswapfailed
  11.Microsoft.Web/sites.SlotSwapWithPreviewStarted: web/sites-slowswapwithpreviewstarted
  12.Microsoft.Web/sites.SlotSwapWithPreviewCancelled: web/sites-slowswapwithpreviewcancelled
  13.Microsoft.Web/sites.AppUpdated.Restarted: web/sites-appupdated-restarted
  14.Microsoft.Web/sites.AppUpdated.Stopped:  web/sites-appupdated-stopped
  15.Microsoft.Web/sites.AppUpdated.ChangedAppSettings:  web/sites-appupdated-changeappsettings
  16.Microsoft.Web/serverfarms.AppServicePlanUpdated: web/serverfarms-appserviceplanupdated
  
Azure Blob Storage Events
  1.Microsoft.Storage.BlobCreated: storage-blobcreated
  2.Microsoft.Storage.BlobDeleted: storage-blobdeleted
  
Azure Container Registry Events
  1.Microsoft.ContainerRegistry.ImagePushed: containerregistery-imagepushed
  2.Microsoft.ContainerRegistry.ImageDeleted: containerregistery-imagedeleted
  3.Microsoft.ContainerRegistry.ChartPushed: containerregistery-chartpushed
  4.Microsoft.ContainerRegistry.ChartDeleted: containerregistery-chartdeleted

Azure Event Hubs
  1.Microsoft.EventHub.CaptureFileCreated: eventhub-capturefilecreated

Azure IoT Hub
  1.Microsoft.Devices.DeviceCreated: devices-devicecreated
  2.Microsoft.Devices.DeviceDeleted: devices-devicedeleted
  3.Microsoft.Devices.DeviceConnected: devices-deviceconnected
  4.Microsoft.Devices.DeviceDisconnected: devices-devicedisconnected
  5.Microsoft.Devices.DeviceTelemetry: devices-devicetelementry

Azure Key Vault
  1.Microsoft.KeyVault.CertificateNewVersionCreated:  keyvault-certificatenewversioncreated, keyvault-certificatenearexpiry, 
  2.Microsoft.KeyVault.CertificateNearExpiry: keyvault-certificatenearexpiry
  3.Microsoft.KeyVault.CertificateExpired: keyvault-certificateexpired
  4.Microsoft.KeyVault.KeyNewVersionCreated: keyvault-keynewversioncreated
  5.Microsoft.KeyVault.KeyNearExpiry: keyvault-keynearexpiry
  6.Microsoft.KeyVault.KeyExpired: keyvault-keyexpired
  7.Microsoft.KeyVault.SecretNewVersionCreated: keyvault-secretnewversioncreated
  8.Microsoft.KeyVault.SecretNearExpiry: keyvault-secretnearexpiry
  9.Microsoft.KeyVault.SecretExpired: keyvault-secretexpired
  
Azure Machine Learning Events
  1.Microsoft.MachineLearningServices.ModelRegistered: machinelearning-modelregistered
  2.Microsoft.MachineLearningServices.ModelDeployed: machinelearning-modeldeployed
  3.Microsoft.MachineLearningServices.RunCompleted: machinelearning-runcompleted
  4.Microsoft.MachineLearningServices.DatasetDriftDetected: machinelearning-datadriftdetected
  5.Microsoft.MachineLearningServices.RunStatusChanged: machinelearning-runstatuschanged

Azure Maps Events
  1.Microsoft.Maps.GeofenceEntere: maps-geofenceentered 
  2.Microsoft.Maps.GeofenceExited: maps-geofenceexited
  3.Microsoft.Maps.GeofenceResult: maps-geofenceresult
  
Azure Media Services Events
  1.Microsoft.Media.JobStateChange: media-jobstatechange
  2.Microsoft.Media.JobScheduled: media-jobscheduled
  3.Microsoft.Media.JobProcessing: media-jobprocessing
  4.Microsoft.Media.JobCanceling: media-jobcanceling
  5.Microsoft.Media.JobFinished: media-jobfinished
  6.Microsoft.Media.JobCanceled: media-jobcanceled
  7.Microsoft.Media.JobErrore: media-joberrored
  8.Microsoft.Media.JobStateChange: media-joboutputstatechange
  9.Microsoft.Media.JobOutputScheduled: media-joboutputscheduled
  10.Microsoft.Media.JobOutputProcessing: media-joboutputprocessing
  11.Microsoft.Media.JobOutputCanceling: media-joboutputcanceling
  12.Microsoft.Media.JobOutputFinished: media-joboutputfinished
  13.Microsoft.Media.JobOutputCanceled: media-joboutputcanceled
  14.Microsoft.Media.JobOutputErrore: media-joboutputerrored

Azure Service Bus Events
  1.Microsoft.ServiceBus.ActiveMessagesAvailableWithNoListeners: servicebus-activemessageavailablewithnolisteners
  2.Microsoft.ServiceBus.DeadletterMessagesAvailableWithNoListener:  servicebus-deadlettermessagesavailablewithnolisteners
  
Azure SignalR Service Events
  1.Microsoft.SignalRService.ClientConnectionConnected: signalrservice-clientconnectionconnected
  2.Microsoft.SignalRService.ClientConnectionDisconnected: signalrservice-clientconnectiondisconnected

```
  
### Example:
#### To trigger the workflow when the Machine Learning model is registered:
```sh

  On:
    repository_dispatch:
        types: [machinelearning-modelregistered]
  (...)

```
