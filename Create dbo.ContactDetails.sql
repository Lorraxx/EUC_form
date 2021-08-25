--Recreate database and table.

USE MASTER
GO
DROP DATABASE IF EXISTS [EUC_form]

CREATE DATABASE EUC_form;

USE [EUC_form]
GO

DROP TABLE IF EXISTS [dbo].[ContactDetails];
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ContactDetails] (
    [ID]                           INT            IDENTITY (1, 1) NOT NULL,
    [Email]                        NVARCHAR (MAX) NOT NULL,
    [PersonalIdentificationNumber] NVARCHAR (11)  NULL,
    [LastName]                     NVARCHAR (50)  NOT NULL,
    [FirstName]                    NVARCHAR (50)  NOT NULL,
    [Nationality]                  INT            NOT NULL,
    [DateOfBirth]                  DATETIME2 (7)  NOT NULL,
    [Gender]                       INT            NOT NULL
);


