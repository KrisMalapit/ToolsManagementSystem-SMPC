IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[V_GroupMembers]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[V_GroupMembers]
AS
select NEWID() AS Id,EmpId as GroupCode,LastName as Members from [dbo].[Employee]
'

