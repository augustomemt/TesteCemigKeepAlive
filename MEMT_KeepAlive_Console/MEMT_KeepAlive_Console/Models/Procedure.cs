using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEMT_KeepAlive.Models
{
    class Procedure
    {
        public string MyProcedure { get; set; } = @"CREATE PROCEDURE [dbo].[sp_MEMT_ChangeServerName] 
  @oldName nvarchar(100), 
  @newName nvarchar(100)
AS
BEGIN
  SET @oldName = UPPER(@oldName)
  SET @newName = UPPER(@newName)
  SET NOCOUNT ON;

  -- update Source table
  UPDATE[ION_Data].[dbo].[Source]
        SET Name = 'LOGINSERTER.' + @newName
  WHERE Name = 'LOGINSERTER.' + @oldName

  UPDATE[ION_Data].[dbo].[Source]
        SET Name = 'QUERYSERVER.' + @newName
  WHERE Name = 'QUERYSERVER.' + @oldName


  DECLARE @Command_ION_Data nvarchar(500)
  IF(CAST((SELECT[Value] FROM [ION_Data].[dbo].[Registry] WHERE Name = 'Revision') AS DECIMAL) > 5.1) 
    BEGIN
      SET @Command_ION_Data = 'UPDATE [ION_Data].[dbo].[Source] SET DisplayName = ''LOGINSERTER.' + @newName + '''' 
      + ' WHERE DisplayName = ''LOGINSERTER.' + @oldName + '''' + ';
      UPDATE[ION_Data].[dbo].[Source] SET DisplayName = ''QUERYSERVER.' + @newName + ''''
      + ' WHERE DisplayName = ''QUERYSERVER.' + @oldName + ''';' 
      EXEC(@Command_ION_Data)
    END

  -- update Machine and Gate tables
  Update[ION_Network].[dbo].[Machine]
        SET name = @newName
  where name = @oldName

  Update[ION_Network].[dbo].[Gate]
        SET name = @newName
  where type = 'Computer'

  -- update CFG_ITemValue table

  UPDATE[ION_Network].[dbo].[CFG_ItemValue]
        SET Value = 'http://' + @newName + '/ionreportdataservice/ReportDataService.asmx'
  WHERE Value = 'http://' + @oldName + '/ionreportdataservice/ReportDataService.asmx'

  -- update IRM_DeviceInfo table

  UPDATE[ION_Network].[dbo].[IRM_DeviceInfo]
        SET Name = 'LOGINSERTER.' + @newName
  WHERE Name = 'LOGINSERTER.' + @oldName

  UPDATE[ION_Network].[dbo].[IRM_DeviceInfo]
        SET Name = 'QUERYSERVER.' + @newName
  WHERE Name = 'QUERYSERVER.' + @oldName

  -- delete Softwarenode table
  -- Network router service will repopulate when it starts.

  DELETE FROM[ION_Network].[dbo].[SoftwareNode]

  -- update SRC_Source
  UPDATE[ION_Network].[dbo].[SRC_Source]
  SET Name = 'LOGINSERTER.' + @newName
  WHERE Name = 'LOGINSERTER.' + @oldName

  UPDATE[ION_Network].[dbo].[SRC_Source]
  SET Name = 'QUERYSERVER.' + @newName
  WHERE Name = 'QUERYSERVER.' + @oldName


  DECLARE @Command_ION_Network nvarchar (500)
  IF(CAST((SELECT[Value] FROM [ION_Network].[dbo].[Registry] WHERE Name = 'Revision') AS DECIMAL) > 13.1) 
    BEGIN
      SET @Command_ION_Network = 'UPDATE [ION_Network].[dbo].[SRC_Source] SET DisplayName = ''LOGINSERTER.' + @newName + '''' 
      + ' WHERE DisplayName = ''LOGINSERTER.' + @oldName + '''' + ';
      UPDATE[ION_Network].[dbo].[SRC_Source] SET DisplayName = ''QUERYSERVER.' + @newName + ''''
      + ' WHERE DisplayName = ''QUERYSERVER.' + @oldName + ''';' 
      EXEC(@Command_ION_Network)
    END

  -- IAS_MeasurementAddress
  DELETE FROM[ION_Network].[dbo].[IAS_MeasurementAddress] WHERE Segment2 like 'VIP%' 
  AND ISManual = 0

  UPDATE[ApplicationModules].[Inventory].[ApplicationServer]
        SET Name = @newName
  WHERE Name = @oldName

  -- Event Watcher tables
  UPDATE[ION_Network].[dbo].[EVT_SourceGroupMap]
        SET SourceName = 'LOGINSERTER.' + @newName
  WHERE SourceName = 'LOGINSERTER.' + @oldName

  -- Limpa lixo nas tabelas ApplicationModules
  DELETE FROM[ApplicationModules].Inventory.HostedService where HostId in (select HostId from[ApplicationModules].Inventory.Host WHERE ServerId  = (select ServerId from[ApplicationModules].[Inventory].[ApplicationServer] where name like '%'+@oldName+'%')) 
  DELETE FROM[ApplicationModules].Inventory.Host WHERE ServerId  = (select ServerId from[ApplicationModules].[Inventory].[ApplicationServer] where name like '%'+@oldName+'%')
  DELETE FROM[ApplicationModules].[Inventory].[ApplicationServer] WHERE Name like '%'+@oldName+'%'

END
";
    }
}
