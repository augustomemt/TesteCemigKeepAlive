using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEMT_KeepAlive.Models
{
    class Tables
    {
        public string KeepTables { get; set; } = @"If not exists (select name from sysobjects where name = 'KeepAction')
CREATE TABLE[dbo].[KeepAction]
        (

   [ActionID][int] NOT NULL,

   [Name] [varchar] (50) NULL,
 CONSTRAINT[PK_KeepAction] PRIMARY KEY CLUSTERED
(
   [ActionID] ASC
)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]
) ON[PRIMARY]

If not exists (select name from sysobjects where name = 'KeepActionStatus')
CREATE TABLE[dbo].[KeepActionStatus]
        (

   [ActionStatusID][int] NOT NULL,

   [Name] [varchar] (50) NULL,
 CONSTRAINT[PK_KeepActionStatus] PRIMARY KEY CLUSTERED
(
   [ActionStatusID] ASC
)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]
) ON[PRIMARY]


If not exists (select name from sysobjects where name = 'KeepALive')
CREATE TABLE[dbo].[KeepAlive]
        (

   [ServerIP][char](20) NOT NULL,

  [ServerPort] [int] NULL,
	[ServerName] [varchar] (50) NULL,
	[ServerBlock] [bit] NULL,
	[ServiceAlive] [bit] NULL,
	[ServiceAliveTime] [datetime] NULL,
	[PrimaryServer] [bit] NULL,
	[Action] [int] NULL,
	[ActionStatus] [int] NULL,
 CONSTRAINT[PK_KeepAlive] PRIMARY KEY CLUSTERED
(
   [ServerIP] ASC
)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]
) ON[PRIMARY]

";
    }
}
