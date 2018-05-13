USE [ElectricalShop]
GO
SET IDENTITY_INSERT [dbo].[Roles] ON 
INSERT [dbo].[Roles] ([RoleId], [RoleName], [Description]) VALUES (10000001, N'Administrators', N'Administrators')
SET IDENTITY_INSERT [dbo].[Roles] OFF
INSERT [dbo].[UserRoles] ([UserId], [RoleId]) VALUES (10000000, 10000001)
INSERT [dbo].[UserRoles] ([UserId], [RoleId]) VALUES (10000000, 10000001)
SET IDENTITY_INSERT [dbo].[Users] ON 

-- user: admin, pass: 123456aA@
INSERT [dbo].[Users] ([UserId], [UserName], [Password], [Salt], [FullName], [Phone], [Email], [IsActive], [CreateDate]) VALUES (10000000, N'admin', N'z38AyXvQ8e+qVRzqQ1FO9cYM6Cp4kO/zJ8bm72/U4hTX/HasWhcpLeDCtH/Vz9tcsBWi/WB9Q4chkzb6YTxeBVS5nUrk/AYeOCaZcMtj3YzjptBH7dCN5jEP7ASpmTEFLHs/MLxk/MeQUbp+nhC6lEPgm8hmGj61Y1i2VpM0nJqIJU+7gtyoxSD+661jMPX6jtreDK9sIzO7qVdrVRRKmlJySCB73/0AFd/b/eCdKVIaT23DlXLeESHJPXrydsqMdnvnEX/tglV8Cp/Q3C6vNRposMLYgZ1xqOHWwFSx1xA4gndaVS2g2Ti8IPcD0cvUtiq25zCh3et52QwCVbpHKQ==', N'3e0BPoMQuAJydJ7ntaT6am3pj7er2nT6PHIqLVs835g=', N'Administrator', N'0909841682', N'locnp.saigon@gmail.com', 1, CAST(N'2017-03-27 21:08:19.493' AS DateTime))
SET IDENTITY_INSERT [dbo].[Users] OFF
