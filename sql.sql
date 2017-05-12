--SELECT * FROM dbo.Users WHERE Users_UID='1CE4AFCE-FA72-4F14-AC87-67F4C59880A5'
go

SELECT a.WfTask_Role,b.User_NTID FROM dbo.Contract_WfTeam a
INNER JOIN  users b
ON  a.Reviewer_UID=b.Users_UID
 WHERE Submitter_UID=(SELECT TOP 1 Users_UID FROM dbo.Users WHERE User_NTID='WAN3L')   ORDER BY 
 CASE
WHEN WfTask_Role='Applicant' THEN  1 
WHEN WfTask_Role='Function Manager I' THEN  2 
WHEN WfTask_Role='Function Manager II' THEN  3 
WHEN WfTask_Role='Purchasing I' THEN  4 
WHEN WfTask_Role='Purchasing II' THEN  5 
WHEN WfTask_Role='SCM I' THEN  6 
WHEN WfTask_Role='SCM II' THEN  7 
WHEN WfTask_Role='Finance I' THEN  8 
WHEN WfTask_Role='Finance II' THEN  9 
WHEN WfTask_Role='Legal Approver I' THEN  10 
WHEN WfTask_Role='Legal Approver II' THEN  11 
WHEN WfTask_Role='Legal Customer I' THEN  12 
WHEN WfTask_Role='Legal Customer II' THEN  13 
WHEN WfTask_Role='Legal Service I' THEN  14 
WHEN WfTask_Role='Legal Service II' THEN  15 
WHEN WfTask_Role='Legal Customer ABC' THEN  16 
WHEN WfTask_Role='Legal Customer NDA' THEN  17 
WHEN WfTask_Role='OPM' THEN  18 
WHEN WfTask_Role='OPD' THEN  19 
WHEN WfTask_Role='OP Assistant' THEN  20 
WHEN WfTask_Role='Upload PIC' THEN  21 
WHEN WfTask_Role='Upload PIC FM' THEN  22 
WHEN WfTask_Role='File In PIC' THEN  23 

END ASC
go
--SELECT COUNT(*)  FROM Contract_WfTeam