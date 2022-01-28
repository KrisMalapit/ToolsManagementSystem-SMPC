IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[V_GroupMembers]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[V_GroupMembers]
AS
SELECT NEWID() AS Id,Code AS GroupCode, EmployeeID AS Members  FROM ( SELECT Code, EmployeeID = STUFF((
 SELECT DISTINCT '', '' + EmployeeID  FROM ( SELECT A.code, C.LastName + '','' + C.FirstName AS EmployeeID  FROM [dbo].[GroupAccountability]A  
 LEFT JOIN [GroupAccountabilityMember]B ON A.id = B.GroupAccountabilityID LEFT JOIN Employee C ON C.id = B.EmployeeID  WHERE B.EmployeeId IS NOT NULL ) b  
 WHERE b.Code = a.Code  FOR XML PATH('''') ) , 1, 2, '''')  FROM (  SELECT A.code, C.LastName + '', '' + C.FirstName AS EmployeeID 
 FROM [dbo].[GroupAccountability]A 
 LEFT JOIN [GroupAccountabilityMember]B ON A.id = B.GroupAccountabilityID 
 LEFT JOIN Employee C ON C.id = B.EmployeeID ) a  GROUP BY Code
)A
'

