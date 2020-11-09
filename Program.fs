module Program

open Pulumi
open Pulumi.FSharp

open Pulumi.Azure.Core
open Pulumi.Azure.Storage

let infra () =

    let location           = "UK South"
    let resourceGroupName  = "rg-bh-repro"
    let storageAccountName = "sabhrepro"
    let containerName      = "c-bh-repro"
    let blobName           = "b-bh-archive"

    let resourceGroup = ResourceGroup (resourceGroupName, ResourceGroupArgs(Location = input location))

    let storageAccount =
        Account(
            storageAccountName,
            AccountArgs(
                ResourceGroupName      = io resourceGroup.Name,
                AccountReplicationType = input "LRS",
                AccountTier            = input "Standard",
                AccountKind            = input "StorageV2",
                Location               = input location))

    let container =
        Container(containerName,
            ContainerArgs
               (StorageAccountName = io storageAccount.Name,
                ContainerAccessType = input "private"))

    let archive = FileArchive("wwwroot") :> AssetOrArchive
    let blob : Blob =
        Blob(blobName,
            BlobArgs
               (StorageAccountName = io storageAccount.Name,
                StorageContainerName = io container.Name,
                Type = input "Block",
                Source = input archive))

    dict []

[<EntryPoint>]
let main _ =
  Deployment.run infra
