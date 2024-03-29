/****** Object:  StoredProcedure [dbo].[GetServiceEndpointMetadata]    Script Date: 7/4/2012 3:03:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetServiceEndpointMetadata]
	@contractType nvarchar(512)
AS
BEGIN

	SELECT Address, BindingType, Binding, UpdatedOn FROM EndpointMetadata WHERE ContractType = @contractType

END

GO
/****** Object:  StoredProcedure [dbo].[RegisterServiceEndpointMetadata]    Script Date: 7/4/2012 3:03:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[RegisterServiceEndpointMetadata]
	@uri nvarchar(256),
	@contractType nvarchar(256),
	@address nvarchar(1024),
	@bindingType nvarchar(512),
	@binding nvarchar(1024)
AS
BEGIN

	BEGIN TRAN

		EXEC UnRegisterServiceEndpointMetadata @uri, @contractType, @address, @bindingType

		INSERT INTO EndpointMetadata (Uri, ContractType, Address, BindingType, Binding) VALUES (@uri, @contractType, @address, @bindingType, @binding)

	COMMIT TRAN

END

GO
/****** Object:  StoredProcedure [dbo].[UnRegisterServiceEndpointMetadata]    Script Date: 7/4/2012 3:03:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UnRegisterServiceEndpointMetadata]
	@uri nvarchar(256),
	@contractType nvarchar(256),
	@address nvarchar(1024),
	@bindingType nvarchar(512)
AS
BEGIN

	DELETE FROM EndpointMetadata WHERE Uri = @uri AND ContractType = @contractType AND Address = @address AND BindingType = @bindingType

END

GO
/****** Object:  Table [dbo].[EndpointMetadata]    Script Date: 7/4/2012 3:03:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EndpointMetadata](
	[Uri] [nvarchar](256) NOT NULL,
	[ContractType] [nvarchar](256) NOT NULL,
	[Address] [nvarchar](1024) NOT NULL,
	[BindingType] [nvarchar](512) NOT NULL,
	[Binding] [nvarchar](1024) NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_EndpointMetadata] PRIMARY KEY CLUSTERED 
(
	[Uri] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_ContractType]    Script Date: 7/4/2012 3:03:01 PM ******/
CREATE NONCLUSTERED INDEX [IX_ContractType] ON [dbo].[EndpointMetadata]
(
	[ContractType] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
ALTER TABLE [dbo].[EndpointMetadata] ADD  CONSTRAINT [DF_EndpiointMetadata_UpdatedOn]  DEFAULT (getutcdate()) FOR [UpdatedOn]
GO
