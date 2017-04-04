using Microsoft.SqlServer.Server;
using SPP.Econtract1._0;
using SPP.Econtract2._0;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SPP.Migration
{
    class Program       
    {
        static void Main(string[] args)
        {
            //Delete_ALL();
            Insert_Tb_Users();
            Insert_Tb_Company();
            Insert_Tb_DepartMent();
            Insert_Tb_Users_CompanyDepartment();
            Insert_Tb_ContractType_M();
            Insert_Tb_ContractType_D();
            Insert_Tb_ContractTemplate();
            Insert_TypeCode_Data();
            Insert_Tb_Module();
            Insert_Contract_M();
            Insert_Tb_Contract_Attachment();
            Insert_Tb_Contract_WfTeam();
            Insert_Tb_WfTaskDelaySetting();
            Insert_Tb_WfDelegation();
            Insert_Tb_WfDelegation_History();
            Insert_Tb_WfEmail_StopExpirationNotice();
            Insert_Tb_WfTask();
            Insert_Tb_WfTask_History();
            //Delete_ALL();
            //test();

        }

        public static Guid modified_guid = new Guid("0B08C006-5AB5-E611-83F5-005056BF221C");
        public static string modify_remarks = "E-Contract 1.0 data migration to E-Contract 2.0";
        public static Guid module_uid = new Guid("39326F1E-54C6-4ED7-9D2D-A0415F8321D3");

        //SELECT* FROM  dbo.Users
        //SELECT* FROM  dbo.Company
        //SELECT* FROM dbo.Department
        //SELECT* FROM Users_CompanyDepartment
        //SELECT* FROM dbo.ContractType_M
        //SELECT* FROM dbo.ContractType_D
        //SELECT* FROM  dbo.ContractTemplate
        //

        public static void Insert_Tb_Users()
        {
            //SELECT* FROM  dbo.Users WHERE User_NTID = 'LiuK9' 数据重复

            //初始化查询
            List<SYSTEM_USERS> users_spp_list = new List<SYSTEM_USERS>();
            using (var context = new SPP_ProductionEntities())
            {
                //string sql = @"
                //    SELECT *,RN FROM
                //    (
                //    SELECT *, ROW_NUMBER() OVER(PARTITION BY ACCOUNT ORDER BY  GETDATE()) AS RN FROM SYSTEM_USERS  WHERE Dept LIKE '%Contract%' or ( Dept IS NULL)
                //    ) t WHERE t.RN = 1
                //                        ";
                //WHERE Dept LIKE '%Contract%' or(Dept IS NULL)or(Account = 'Lia9')
                string sql = @"
                    SELECT *,RN FROM
                    (
                    SELECT *, ROW_NUMBER() OVER(PARTITION BY ACCOUNT ORDER BY  GETDATE()) AS RN FROM SYSTEM_USERS    WHERE    account<>'yangy9'  and  account<>'YangY225'
                    ) t WHERE t.RN = 1
                                        ";
                //account <> 'LiuK9'  and
                 users_spp_list = context.SYSTEM_USERS.SqlQuery(sql).ToList();

            }

            //已存在ntid
            List<string> users_sppmvc_list = new List<string>();
            using (var context = new SPP_MVC_Entities())
            {
                users_sppmvc_list = context.Users.Select(m => m.User_NTID.ToLower()).ToList<string>();
            }

            //去除已存在的ntid
            users_spp_list = users_spp_list.Where(m => !users_sppmvc_list.Contains(m.ACCOUNT.ToLower())).ToList();


            //插入数据.
            List<Users> users_list = new List<Users>();
            if (users_spp_list.Count > 0)
            {
                using (var context = new SPP_MVC_Entities())
                {
                    foreach (var item in users_spp_list)
                    {

                        Users model_Users = new Users();
                        model_Users.Users_UID = Guid.NewGuid();
                        model_Users.User_NTID = item.ACCOUNT;
                        model_Users.User_Name = item.USER_NAME;
                        model_Users.Is_Enable = true;
                        model_Users.Email = item.EMAIL;
                        model_Users.Tel = item.JVN_TEL;
                        model_Users.Login_Token = null;
                        model_Users.Modified_UID = modified_guid;
                        model_Users.Modified_Date = DateTime.Now;
                        model_Users.Modified_Remarks = modify_remarks;
                        users_list.Add(model_Users);
                    }
                    context.Users.AddRange(users_list);
                    context.SaveChanges();
                }

            }


        }

        public static void Insert_Tb_Company()
        {
            List<SYSTEM_PLANT> system_plant_spp_list = new List<SYSTEM_PLANT>();
            using (var context = new SPP_ProductionEntities())
            {

                //var sql = @"
                //          SELECT * FROM
                //          (
                //          SELECT * ,ROW_NUMBER() OVER (PARTITION BY  CCODE ORDER BY  GETDATE() ) AS RN FROM dbo.SYSTEM_PLANT
                //          ) t WHERE t.RN=1
                //          ";

                var sql = @"
    SELECT * FROM
                          (
                          SELECT * ,ROW_NUMBER() OVER (PARTITION BY  CCODE ORDER BY  GETDATE() ) AS RN FROM dbo.SYSTEM_PLANT
                          ) t WHERE t.RN=1 AND  t.NAME_2 LIKE '%E-Contract%'
";

                system_plant_spp_list = context.SYSTEM_PLANT.SqlQuery(sql).ToList();

            }

            //已存在CompanyCode
            List<string> company_sppmvc_list = new List<string>();
            using (var context = new SPP_MVC_Entities())
            {
                company_sppmvc_list = context.Company.Select(m => m.Company_Code).ToList<string>();
            }
            //去除已存在的CompanyCode
            system_plant_spp_list = system_plant_spp_list.Where(m => !company_sppmvc_list.Contains(m.CCODE)).ToList();

            //插入数据
            List<Company> comany_list = new List<Company>();
            using (var context = new SPP_MVC_Entities())
            {
                foreach (var item in system_plant_spp_list)
                {
                    Company model_Company = new Company();
                    model_Company.Company_UID = Guid.NewGuid();
                    model_Company.Company_Code = item.CCODE;
                    model_Company.Company_Name_ZH = item.LEGAL_ENTITY_ZH;
                    model_Company.Company_Name_EN = item.LEGAL_ENTITY_EN;
                    model_Company.Address_ZH = item.ADDRESS_ZH;
                    model_Company.Address_EN = item.ADDRESS_EN;
                    model_Company.Is_Enable = true;
                    model_Company.Modified_UID = modified_guid;
                    model_Company.Modified_Date = DateTime.Now;
                    model_Company.Modified_Remarks = modify_remarks;
                    comany_list.Add(model_Company);
                }
                context.Company.AddRange(comany_list);
                context.SaveChanges();
            }



        }

        public static void Insert_Tb_DepartMent()
        {

            //SITE_CODE    DEPARTMENT Company_Code    Department_UID Company_UID 关系 旧架构
            //            var sql = @"
            //SELECT  DISTINCT a.SITE_CODE,a.DEPARTMENT,b.Company_Code,
            //'95BCBAB5-544C-465A-A6F1-4B9DBD'+ RIGHT('000000'+CAST(ROW_NUMBER()OVER( ORDER BY GETDATE() ) AS nvarchar(50)),6 )
            //AS Department_UID,
            //'' AS Company_UID 
            //FROM WF_REVIEW_TEAM_CONTRACT_SITE a
            //LEFT JOIN 
            //(
            //select distinct NAME_0 as SITE_CODE , CCODE  as Company_Code  from SYSTEM_PLANT, SYSTEM_USER_PLANT
            //where SYSTEM_PLANT.LOCATION = SYSTEM_USER_PLANT.PLANT_LOCATION
            //and SYSTEM_PLANT.TYPE = SYSTEM_USER_PLANT.PLANT_TYPE
            //) b ON  a.SITE_CODE=b.SITE_CODE GROUP BY  a.SITE_CODE,a.DEPARTMENT,b.Company_Code    ORDER BY  a.SITE_CODE,DEPARTMENT
            //";

            //            var sql = @"
            //SELECT DISTINCT a.SITE_CODE ,a.DEPARTMENT  ,b.Company_Code,
            //'95BCBAB5-544C-465A-A6F1-4B9DBD'+ RIGHT('000000'+CAST(ROW_NUMBER()OVER( ORDER BY GETDATE() ) AS nvarchar(50)),6 )
            //AS Department_UID,
            //'' AS Company_UID 
            //FROM dbo.SYSTEM_DEPARTMENT a
            //LEFT JOIN  
            //(
            //select distinct NAME_0 as SITE_CODE , CCODE  as Company_Code  from SYSTEM_PLANT, SYSTEM_USER_PLANT
            //where SYSTEM_PLANT.LOCATION = SYSTEM_USER_PLANT.PLANT_LOCATION
            //and SYSTEM_PLANT.TYPE = SYSTEM_USER_PLANT.PLANT_TYPE
            //) b ON  a.SITE_CODE=b.SITE_CODE GROUP BY  a.SITE_CODE,a.DEPARTMENT,b.Company_Code    ORDER BY  a.SITE_CODE,DEPARTMENT
            //";

            //            var sql = @"
            //		SELECT DISTINCT SITE_NAME AS SITE_CODE ,DEPARTMENT AS DEPARTMENT,
            //			b.Company_Code,
            //'95BCBAB5-544C-465A-A6F1-4B9DBD'+ RIGHT('000000'+CAST(ROW_NUMBER()OVER( ORDER BY GETDATE() ) AS nvarchar(50)),6 )
            //AS Department_UID,
            //'' AS Company_UID 
            //			 FROM 
            //			(
            //			SELECT DISTINCT SITE_NAME,DEPARTMENT FROM  dbo.SYSTEM_USER_DEPARTMENT
            //			UNION ALL
            //			SELECT DISTINCT SITE_CODE,DEPARTMENT FROM dbo.SYSTEM_DEPARTMENT
            //			)a 
            //			LEFT JOIN  
            //(
            //select distinct NAME_0 as SITE_CODE , CCODE  as Company_Code  from SYSTEM_PLANT, SYSTEM_USER_PLANT
            //where SYSTEM_PLANT.LOCATION = SYSTEM_USER_PLANT.PLANT_LOCATION
            //and SYSTEM_PLANT.TYPE = SYSTEM_USER_PLANT.PLANT_TYPE
            //) b ON  a.SITE_NAME=b.SITE_CODE GROUP BY  a.SITE_NAME,a.DEPARTMENT,b.Company_Code    ORDER BY  a.SITE_NAME,DEPARTMENT
            //";

            var sql = @"
	SELECT DISTINCT SITE_NAME AS SITE_CODE ,DEPARTMENT AS DEPARTMENT,
			b.Company_Code,
'95BCBAB5-544C-465A-A6F1-4B9DBD'+ RIGHT('000000'+CAST(ROW_NUMBER()OVER( ORDER BY GETDATE() ) AS nvarchar(50)),6 )
AS Department_UID,
'' AS Company_UID 
			 FROM 
			(
		     SELECT DISTINCT SITE_NAME,DEPARTMENT FROM  dbo.SYSTEM_USER_DEPARTMENT
			UNION ALL
			SELECT DISTINCT SITE_CODE,DEPARTMENT FROM dbo.SYSTEM_DEPARTMENT
			UNION ALL
			SELECT DISTINCT SITE_CODE,DEPARTMENT FROM  dbo.CONTRACT_M
			UNION ALL
			SELECT DISTINCT SITE_CODE,DEPARTMENT FROM dbo.WF_REVIEW_TEAM_CONTRACT_SITE
			)a 
			LEFT JOIN  
(
select distinct NAME_0 as SITE_CODE , CCODE  as Company_Code  from SYSTEM_PLANT, SYSTEM_USER_PLANT
where SYSTEM_PLANT.LOCATION = SYSTEM_USER_PLANT.PLANT_LOCATION
and SYSTEM_PLANT.TYPE = SYSTEM_USER_PLANT.PLANT_TYPE
) b ON  a.SITE_NAME=b.SITE_CODE GROUP BY  a.SITE_NAME,a.DEPARTMENT,b.Company_Code    ORDER BY  a.SITE_NAME,DEPARTMENT
";


            List<Insert_Tb_DepartMent_1> insert_tb_department_1_list = new List<Insert_Tb_DepartMent_1>();

            List<Company> sppmvc_company_list = new List<Company>();
            using (var context = new SPP_MVC_Entities())
            {
                sppmvc_company_list = context.Company.ToList();
            }


            //Company_UID 赋值
            using (var context = new SPP_ProductionEntities())
            {
                insert_tb_department_1_list = context.Database.SqlQuery<Insert_Tb_DepartMent_1>(sql).ToList();
                foreach (var item in insert_tb_department_1_list)
                {
                    item.Company_UID = sppmvc_company_list.Where(m => m.Company_Code == item.Company_Code).FirstOrDefault().Company_UID.ToString();
                }
            }

            //新增
            using (var context = new SPP_MVC_Entities())
            {
                List<Department> department_list = new List<Department>();
                foreach (var item in insert_tb_department_1_list)
                {
                    Department model_Department = new Department();
                    model_Department.Department_UID = new Guid(item.Department_UID);
                    model_Department.Company_UID = new Guid(item.Company_UID);
                    model_Department.SAP_CostCenter = String.Empty;
                    model_Department.Department_Name = item.DEPARTMENT;
                    model_Department.Is_Enable = true;
                    model_Department.Modified_UID = modified_guid;
                    model_Department.Modified_Date = DateTime.Now;
                    model_Department.Modified_Remarks = modify_remarks;
                    department_list.Add(model_Department);
                }
                context.Department.AddRange(department_list);
                context.SaveChanges();
            }



        }

        public static void Insert_Tb_Users_CompanyDepartment()
        {
            //            string sql = @"
            //SELECT  DISTINCT a.SITE_CODE,a.DEPARTMENT,a.SUBMIT,b.Company_Code,
            //''
            //AS Department_UID,
            //'' AS Company_UID ,

            //NEWID() AS Users_CompanyDepartment_UID ,
            //'' AS Users_UID
            //FROM WF_REVIEW_TEAM_CONTRACT_SITE a
            //LEFT JOIN 
            //(
            //select distinct NAME_0 as SITE_CODE , CCODE  as Company_Code  from SYSTEM_PLANT, SYSTEM_USER_PLANT
            //where SYSTEM_PLANT.LOCATION = SYSTEM_USER_PLANT.PLANT_LOCATION
            //and SYSTEM_PLANT.TYPE = SYSTEM_USER_PLANT.PLANT_TYPE
            //) b ON  a.SITE_CODE=b.SITE_CODE GROUP BY  a.SITE_CODE,a.DEPARTMENT,b.Company_Code ,a.SUBMIT   ORDER BY  a.SITE_CODE,DEPARTMENT

            //";
            string sql = @"
SELECT  DISTINCT a.SITE_NAME AS SITE_CODE,a.DEPARTMENT,a.ACCOUNT as SUBMIT,b.Company_Code,
''
AS Department_UID,
'' AS Company_UID ,

NEWID() AS Users_CompanyDepartment_UID ,
'' AS Users_UID
FROM SYSTEM_User_DEPARTMENT a
LEFT JOIN 
(
select distinct NAME_0 as SITE_CODE , CCODE  as Company_Code  from SYSTEM_PLANT, SYSTEM_USER_PLANT
where SYSTEM_PLANT.LOCATION = SYSTEM_USER_PLANT.PLANT_LOCATION
and SYSTEM_PLANT.TYPE = SYSTEM_USER_PLANT.PLANT_TYPE
) b ON  a.SITE_NAME=b.SITE_CODE GROUP BY  a.SITE_NAME,a.DEPARTMENT,b.Company_Code ,a.ACCOUNT   ORDER BY  a.SITE_NAME,DEPARTMENT
";

            List<Department> sppmvc_department_list = new List<Department>();
            List<Company> sppmvc_company_list = new List<Company>();
            List<Users> sppmvc_users_list = new List<Users>();
            using (var context = new SPP_MVC_Entities())
            {
                sppmvc_department_list = context.Department.ToList();
                sppmvc_company_list = context.Company.ToList();
                sppmvc_users_list = context.Users.ToList();

            }

            List<Insert_Tb_Users_CompanyDepartment_1> insert_tb_users_companydepartment = new List<Insert_Tb_Users_CompanyDepartment_1>();
            using (var context = new SPP_ProductionEntities())
            {
                insert_tb_users_companydepartment = context.Database.SqlQuery<Insert_Tb_Users_CompanyDepartment_1>(sql).ToList();
                //赋值Company_UID
                foreach (var item in insert_tb_users_companydepartment)
                {
                    item.Company_UID = sppmvc_company_list.Where(m => m.Company_Code == item.Company_Code).FirstOrDefault().Company_UID.ToString();

                }
                //根据 赋值Company_UID，DepartMentName 赋值 Department_UID  .根据NTID 赋值Users_UID
                foreach (var item in insert_tb_users_companydepartment)
                {

                    var depart = sppmvc_department_list.Where(m => m.Company_UID == new Guid(item.Company_UID) & m.Department_Name == item.DEPARTMENT).FirstOrDefault();
                    if (depart != null)
                    {
                        item.Department_UID = depart.Department_UID.ToString();
                        item.Users_UID = sppmvc_users_list.Where(m => m.User_NTID.ToLower() == item.SUBMIT.ToLower()).FirstOrDefault().Users_UID.ToString();

                    }
                    else
                    {
                        item.Department_UID = null;
                    }

                }
            }
            //过滤不存在 的 insert_tb_users_companydepartment
            insert_tb_users_companydepartment = insert_tb_users_companydepartment.Where(m => m.Department_UID != null).ToList();
            using (var context = new SPP_MVC_Entities())
            {
                //插入 Users_CompanyDepartment
                List<Users_CompanyDepartment> Users_CompanyDepartment_list = new List<Users_CompanyDepartment>();
                foreach (var item in insert_tb_users_companydepartment)
                {
                    Users_CompanyDepartment model_Users_CompanyDepartment = new Users_CompanyDepartment();
                    model_Users_CompanyDepartment.Users_CompanyDepartment_UID = item.Users_CompanyDepartment_UID;
                    model_Users_CompanyDepartment.Users_UID = new Guid(item.Users_UID);
                    model_Users_CompanyDepartment.Company_UID = new Guid(item.Company_UID);
                    model_Users_CompanyDepartment.Department_UID = new Guid(item.Department_UID);
                    model_Users_CompanyDepartment.Begin_Date = DateTime.Now;
                    model_Users_CompanyDepartment.End_Date = null;
                    model_Users_CompanyDepartment.Modified_UID = modified_guid;
                    model_Users_CompanyDepartment.Modified_Date = DateTime.Now;
                    model_Users_CompanyDepartment.Modified_Remarks = modify_remarks;
                    Users_CompanyDepartment_list.Add(model_Users_CompanyDepartment);

                }
                context.Users_CompanyDepartment.AddRange(Users_CompanyDepartment_list);
                context.SaveChanges();

            }

        }

        public static void Insert_Tb_ContractType_M()
        {
            Delete_ContractType();

            List<string> contracttype_m_str = new List<string>();
            List<ContractType_M> contract_type_m_list = new List<ContractType_M>();
            using (var context = new SPP_ProductionEntities())
            {
                var sql = @"SELECT DISTINCT TYPE_CATEGORY FROM CONTRACT_TYPE_MASTER";
                contracttype_m_str = context.Database.SqlQuery<string>(sql).ToList();

                foreach (var item in contracttype_m_str)
                {
                    ContractType_M model_ContractType_M = new ContractType_M();
                    model_ContractType_M.ContractType_M_UID = Guid.NewGuid();
                    model_ContractType_M.ContractType_M_Name = item.ToString();
                    model_ContractType_M.Is_Enable = true;
                    model_ContractType_M.Modified_UID = modified_guid;
                    model_ContractType_M.Modified_Date = DateTime.Now;
                    model_ContractType_M.Modified_Remarks = modify_remarks;
                    contract_type_m_list.Add(model_ContractType_M);
                }

            }
            using (var context = new SPP_MVC_Entities())
            {

                context.ContractType_M.AddRange(contract_type_m_list);
                context.SaveChanges();
            }
        }

        public static void Insert_Tb_ContractType_D()
        {

            List<ContractType_M> ContractType_M_list = new List<ContractType_M>();
            List<CONTRACT_TYPE_MASTER> CONTRACT_TYPE_MASTER_list = new List<CONTRACT_TYPE_MASTER>();

            using (var context = new SPP_ProductionEntities())
            {
                CONTRACT_TYPE_MASTER_list = context.CONTRACT_TYPE_MASTER.ToList();

            }
            List<ContractType_D> ContractType_D_list = new List<ContractType_D>();

            using (var context = new SPP_MVC_Entities())
            {
                ContractType_M_list = context.ContractType_M.ToList();
                foreach (var item in CONTRACT_TYPE_MASTER_list)
                {
                    ContractType_D model_ContractType_D = new ContractType_D();
                    model_ContractType_D.ContractType_D_UID = Guid.NewGuid();
                    model_ContractType_D.ContractType_M_UID = ContractType_M_list.Where(m => m.ContractType_M_Name == item.TYPE_CATEGORY).FirstOrDefault().ContractType_M_UID;
                    model_ContractType_D.ContractType_D_Name_EN = item.TYPE_CODE;
                    model_ContractType_D.ContractType_D_Name_ZH = item.TEMPLATE_NOTE;
                    model_ContractType_D.Period_Required = item.PERIOD_REQUIRED.HasValue ? Convert.ToBoolean(item.PERIOD_REQUIRED) : false;
                    model_ContractType_D.Need_Finance = item.ISFINANCEREVIEW.HasValue ? Convert.ToBoolean(item.ISFINANCEREVIEW) : false;
                    model_ContractType_D.Need_Purchasing = item.ISPURREVIEW.HasValue ? Convert.ToBoolean(item.ISPURREVIEW) : false;
                    model_ContractType_D.Need_SupplyChain = item.ISSCMREVIEW.HasValue ? Convert.ToBoolean(item.ISSCMREVIEW) : false;
                    model_ContractType_D.Need_Legal_General = item.ONLYLEGALREVIEW.HasValue ? Convert.ToBoolean(item.ONLYLEGALREVIEW) : false;
                    model_ContractType_D.Need_Legal_CustomerABC = false;
                    model_ContractType_D.Need_Legal_CustomerNDA = false;
                    model_ContractType_D.Need_Legal_Customer = false;
                    model_ContractType_D.Need_Legal_Service = false;
                    model_ContractType_D.Is_Enable = true;
                    model_ContractType_D.Modified_UID = modified_guid;
                    model_ContractType_D.Modified_Date = DateTime.Now;
                    model_ContractType_D.Modified_Remarks = modify_remarks;
                    ContractType_D_list.Add(model_ContractType_D);
                }
                context.ContractType_D.AddRange(ContractType_D_list);
                context.SaveChanges();
            }

        }

        public static void Insert_Tb_ContractTemplate()
        {
            List<Insert_Tb_ContractTemplate_1> insert_tb_contracttemplate_1 = new List<Insert_Tb_ContractTemplate_1>();
            List<Company> company_list = new List<Company>();
            List<ContractType_D> contract_d_list = new List<ContractType_D>();

            List<ContractType_M> contract_type_m_list = new List<ContractType_M>();
            using (var context = new SPP_MVC_Entities())
            {
                company_list = context.Company.ToList();
                contract_type_m_list = context.ContractType_M.ToList();
                contract_d_list = context.ContractType_D.ToList();


            }

            using (var context = new SPP_ProductionEntities())
            {
                //过滤重复部门的表单
                var sql = @"
SELECT * FROM
(
SELECT [CONTRACT_TYPE_UID], [TYPE_GROUP], [TYPE_CODE], [TEMPLATE_NAME], [TEMPLATE_PATH], [TEMPLATE_DESC], [VERSION], [DEL_MK], [CREATOR], [CREATE_DATE], [MODIFIER], [MODIFY_DATE], [PLANT_LOCATION], [PLANT_TYPE], [ONLYLEGALREVIEW], [PERIOD_REQUIRED], [TEMPLATE_NOTE], [ISFINANCEREVIEW], [ISPURREVIEW], [ISSCMREVIEW], [TYPE_CATEGORY] 
,b.CCODE,'' AS Company_UID,'' AS ContractType_M_UID,ROW_NUMBER() OVER (PARTITION BY  TYPE_CODE,CCODE,TEMPLATE_NAME ORDER BY GETDATE()) AS RN
 FROM (SELECT * FROM  dbo.CONTRACT_TYPE WHERE LEN(TEMPLATE_NAME)>10 ) a
LEFT JOIN dbo.SYSTEM_PLANT b
ON a.PLANT_LOCATION=b.LOCATION AND  a.PLANT_TYPE=b.TYPE
) t WHERE t.RN=1
           ";
                insert_tb_contracttemplate_1 = context.Database.SqlQuery<Insert_Tb_ContractTemplate_1>(sql).ToList();

                //赋值Company_UID、CONTRACT_TYPE_UID
                foreach (var item in insert_tb_contracttemplate_1)
                {
                    item.Company_UID = company_list.Where(m => m.Company_Code == item.CCODE).FirstOrDefault().Company_UID.ToString();
                    //item.CONTRACT_TYPE_UID = contract_type_m_list.Where(m => m.ContractType_M_Name == item.TYPE_CATEGORY).FirstOrDefault().ContractType_M_UID.ToString();
                    item.CONTRACT_TYPE_UID = contract_d_list.Where(m => m.ContractType_D_Name_EN == item.TYPE_CODE).FirstOrDefault().ContractType_D_UID.ToString();
                }
            }

            using (var context = new SPP_MVC_Entities())
            {
                List<ContractTemplate> ContractTemplate_list = new List<ContractTemplate>();
                foreach (var item in insert_tb_contracttemplate_1)
                {
                    ContractTemplate model_ContractTemplate = new ContractTemplate();
                    model_ContractTemplate.ContractTemplate_UID = Guid.NewGuid();
                    model_ContractTemplate.Company_UID = new Guid(item.Company_UID);
                    model_ContractTemplate.ContractType_D_UID = new Guid(item.CONTRACT_TYPE_UID);
                    model_ContractTemplate.System_File_Name = "";
                    model_ContractTemplate.Original_File_Name = item.TEMPLATE_NAME;
                    model_ContractTemplate.Display_File_Name = item.TEMPLATE_NAME;
                    model_ContractTemplate.File_Size = 0;
                    model_ContractTemplate.File_Path = "";
                    model_ContractTemplate.Tempkey = Guid.Empty;
                    if (item.DEL_MK == "Y")
                    {
                        model_ContractTemplate.Is_Enable = false;
                    }
                    else
                    {
                        model_ContractTemplate.Is_Enable = true;
                    }
                    model_ContractTemplate.Modified_UID = modified_guid;
                    model_ContractTemplate.Modified_Date = DateTime.Now;
                    model_ContractTemplate.Modified_Remarks = item.TEMPLATE_DESC;

                    ContractTemplate_list.Add(model_ContractTemplate);
                }
                ContractTemplate_list.Count();

                context.ContractTemplate.AddRange(ContractTemplate_list);
                context.SaveChanges();

            }


        }

        public static void Delete_ALL()
        {
            using (var context = new SPP_MVC_Entities())
            {
                var contract_m = context.Contract_M.ToList();
                var users = context.Users.Where(m => m.Modified_Remarks == modify_remarks).ToList();
                var company = context.Company.Where(m => m.Modified_Remarks == modify_remarks).ToList();
                var department = context.Department.Where(m => m.Modified_Remarks == modify_remarks).ToList();
                var users_company_department = context.Users_CompanyDepartment.Where(m => m.Modified_Remarks == modify_remarks).ToList();
                var contracttype_m = context.ContractType_M.ToList();
                var contracttype_d = context.ContractType_D.ToList();
                var contracttemplate = context.ContractTemplate.ToList();


                context.Contract_M.RemoveRange(contract_m);
                context.Users.RemoveRange(users);
                context.Company.RemoveRange(company);
                context.Department.RemoveRange(department);
                context.Users_CompanyDepartment.RemoveRange(users_company_department);
                context.ContractTemplate.RemoveRange(contracttemplate);
                context.ContractType_D.RemoveRange(contracttype_d);
                context.ContractType_M.RemoveRange(contracttype_m);
                context.SaveChanges();
            }
        }

        public static void Delete_ContractType()
        {
            using (var context = new SPP_MVC_Entities())
            {
                var contracttype_m = context.ContractType_M.ToList();
                var contracttype_d = context.ContractType_D.ToList();
                var contracttemplate = context.ContractTemplate.ToList();
                var contract_m = context.Contract_M.ToList();
                var contract_d = context.Contract_Attachment.ToList();

                context.Contract_Attachment.RemoveRange(contract_d);
                context.SaveChanges();
                context.Contract_M.RemoveRange(contract_m);
                context.SaveChanges();
                context.ContractTemplate.RemoveRange(contracttemplate);
                context.SaveChanges();
                context.ContractType_D.RemoveRange(contracttype_d);
                context.SaveChanges();
                context.ContractType_M.RemoveRange(contracttype_m);
                context.SaveChanges();
            }



        }

        public static void Insert_TypeCode_Data()
        {
            Guid renew_guid = new Guid("E705C006-5AB5-E611-83F5-005056BF221C");
            TypeCode_L1 model_TypeCode_L1 = new TypeCode_L1();
            model_TypeCode_L1.TypeCode_L1_UID = renew_guid;
            model_TypeCode_L1.TypeCode_L1_ID = "S013";
            model_TypeCode_L1.TypeCode_L1_Name = "Contract IsRenew";
            model_TypeCode_L1.Begin_Date = DateTime.Now;
            model_TypeCode_L1.End_Date = null;
            model_TypeCode_L1.Reserved_1 = null;
            model_TypeCode_L1.Reserved_2 = null;
            model_TypeCode_L1.Remarks = null;
            model_TypeCode_L1.Modified_UID = modified_guid;
            model_TypeCode_L1.Modified_Date = DateTime.Now;
            model_TypeCode_L1.Modified_Remarks = null;

            List<TypeCode_L2> TypeCode_L2_list = new List<TypeCode_L2>();

            {
                TypeCode_L2 model_TypeCode_L2 = new TypeCode_L2();
                model_TypeCode_L2.TypeCode_L2_UID = new Guid("E9AFA82A-BCFA-46BE-8A8A-E13F74B4FF59");
                model_TypeCode_L2.TypeCode_L1_UID = renew_guid;
                model_TypeCode_L2.TypeCode_L2_ID = "S013-01";
                model_TypeCode_L2.TypeCode_L2_Name = "New Contract";
                model_TypeCode_L2.Begin_Date = DateTime.Now;
                model_TypeCode_L2.End_Date = null;
                model_TypeCode_L2.Reserved_1 = null;
                model_TypeCode_L2.Reserved_2 = null;
                model_TypeCode_L2.Remarks = null;
                model_TypeCode_L2.Modified_UID = modified_guid;
                model_TypeCode_L2.Modified_Date = DateTime.Now;
                model_TypeCode_L2.Modified_Remarks = null;
                TypeCode_L2_list.Add(model_TypeCode_L2);
            }

            {
                TypeCode_L2 model_TypeCode_L2 = new TypeCode_L2();
                model_TypeCode_L2.TypeCode_L2_UID = new Guid("13A6D3EE-ED95-4EC2-9E6E-C21E8131A9AB");
                model_TypeCode_L2.TypeCode_L1_UID = renew_guid;
                model_TypeCode_L2.TypeCode_L2_ID = "S013-02";
                model_TypeCode_L2.TypeCode_L2_Name = "Renew with same contract";
                model_TypeCode_L2.Begin_Date = DateTime.Now;
                model_TypeCode_L2.End_Date = null;
                model_TypeCode_L2.Reserved_1 = null;
                model_TypeCode_L2.Reserved_2 = null;
                model_TypeCode_L2.Remarks = null;
                model_TypeCode_L2.Modified_UID = modified_guid;
                model_TypeCode_L2.Modified_Date = DateTime.Now;
                model_TypeCode_L2.Modified_Remarks = null;
                TypeCode_L2_list.Add(model_TypeCode_L2);
            }

            {
                TypeCode_L2 model_TypeCode_L2 = new TypeCode_L2();
                model_TypeCode_L2.TypeCode_L2_UID = new Guid("523D5162-EE87-4E16-9474-0FFCB89242DF");
                model_TypeCode_L2.TypeCode_L1_UID = renew_guid;
                model_TypeCode_L2.TypeCode_L2_ID = "S013-03";
                model_TypeCode_L2.TypeCode_L2_Name = "Renew with differ contract";
                model_TypeCode_L2.Begin_Date = DateTime.Now;
                model_TypeCode_L2.End_Date = null;
                model_TypeCode_L2.Reserved_1 = null;
                model_TypeCode_L2.Reserved_2 = null;
                model_TypeCode_L2.Remarks = null;
                model_TypeCode_L2.Modified_UID = modified_guid;
                model_TypeCode_L2.Modified_Date = DateTime.Now;
                model_TypeCode_L2.Modified_Remarks = null;
                TypeCode_L2_list.Add(model_TypeCode_L2);
            }

            using (var context = new SPP_MVC_Entities())
            {

                var type_code1_model = context.TypeCode_L1.Where(m => m.TypeCode_L1_UID == renew_guid).ToList();
                if (type_code1_model.Count > 0)
                {
                    context.TypeCode_L1.RemoveRange(type_code1_model);
                }

                var type_code2_list = context.TypeCode_L2.Where(m => m.TypeCode_L1_UID == renew_guid).ToList();
                if (type_code2_list.Count > 0)
                {
                    context.TypeCode_L2.RemoveRange(type_code2_list);
                }
                context.SaveChanges();
                context.TypeCode_L1.Add(model_TypeCode_L1);
                context.TypeCode_L2.AddRange(TypeCode_L2_list);
                context.SaveChanges();

            }



        }

        public static void Insert_Tb_Module()
        {

            using (var context = new SPP_MVC_Entities())
            {
                Module model_Module = new Module();
                model_Module.Module_UID = module_uid;
                model_Module.Module_Name = "E-Contract";
                model_Module.Users_PIC_UIDs = "1F08C006-5AB5-E611-83F5-005056BF221C";
                model_Module.System_PIC_UIDs = "3908C006-5AB5-E611-83F5-005056BF221C,4A08C006-5AB5-E611-83F5-005056BF221C,0B08C006-5AB5-E611-83F5-005056BF221C,1608C006-5AB5-E611-83F5-005056BF221C";
                model_Module.Is_Enable = true;
                model_Module.Modified_UID =modified_guid;
                model_Module.Modified_Date = DateTime.Now;
                model_Module.Modified_Remarks = "Users: Amber; System: CM, Amanda, Hongzhong, Eugene";
                context.Module.Add(model_Module);
                context.SaveChanges();
            }

        }

        public static void Insert_Contract_M()
        {

            List<Users> users = new List<Users>();
            List<Company> company = new List<Company>();
            List<Department> department = new List<Department>();
            List<ContractType_D> contract_d = new List<ContractType_D>();
            List<TypeCode_L2> typecode = new List<TypeCode_L2>();
            using (var context = new SPP_MVC_Entities())
            {
                users = context.Users.ToList();
                company = context.Company.ToList();
                department = context.Department.ToList();
                contract_d = context.ContractType_D.ToList();
                typecode = context.TypeCode_L2.ToList();
            }

            //            var sql = @"
            //                SELECT a.* ,b.Company_Code,'' AS Applicant_UID,'' AS Department_UID,'' AS ContractType_D_UID,'' AS Is_Renew_UID,'' as JabilEntity_UID  FROM dbo.CONTRACT_M a LEFT JOIN  (select distinct NAME_0 as SITE_CODE, CCODE as Company_Code  from SYSTEM_PLANT, SYSTEM_USER_PLANT
            //where SYSTEM_PLANT.LOCATION = SYSTEM_USER_PLANT.PLANT_LOCATION
            //and SYSTEM_PLANT.TYPE = SYSTEM_USER_PLANT.PLANT_TYPE) b
            //ON a.SITE_CODE = b.SITE_CODE WHERE a.CONTRACT_TYPE<>'' 
            //AND CONTRACT_NO NOT IN ('09361602031','09361604008','09361604028','09361604030','GPB1512002','HUA-2015-0393-IE')
            //                ";

            //            var sql = @"
            //           SELECT   a.* ,
            //                    b.Company_Code ,
            //                    '' AS Applicant_UID ,
            //                    '' AS Department_UID ,
            //                    '' AS ContractType_D_UID ,
            //                    '' AS Is_Renew_UID ,
            //                    '' AS JabilEntity_UID,
            //                    c.NT_ACCOUNT  AS Modify_NTID
            //           FROM     dbo.CONTRACT_M a
            //           LEFT JOIN ( SELECT DISTINCT
            //                                NAME_0 AS SITE_CODE ,
            //                                CCODE AS Company_Code
            //                       FROM     SYSTEM_PLANT ,
            //                                SYSTEM_USER_PLANT
            //                       WHERE    SYSTEM_PLANT.LOCATION = SYSTEM_USER_PLANT.PLANT_LOCATION AND SYSTEM_PLANT.TYPE = SYSTEM_USER_PLANT.PLANT_TYPE ) b ON a.SITE_CODE = b.SITE_CODE
            //           LEFT JOIN dbo.CHANGE_HISTORY c
            //           ON  a.CONTRACT_M_UID=c.OBJ_UID
            //           WHERE    a.CONTRACT_TYPE <> '' AND CONTRACT_NO NOT IN ( '09361602031', '09361604008', '09361604028', '09361604030', 'GPB1512002', 'HUA-2015-0393-IE' )  AND  c.NT_ACCOUNT IS NOT NULL

            //";

            var sql = @"
           SELECT   a.* ,
                    b.Company_Code ,
                    '' AS Applicant_UID ,
                    '' AS Department_UID ,
                    '' AS ContractType_D_UID ,
                    '' AS Is_Renew_UID ,
                    '' AS JabilEntity_UID,
                    c.NT_ACCOUNT  AS Modify_NTID
           FROM     dbo.CONTRACT_M a
           LEFT JOIN ( SELECT DISTINCT
                                NAME_0 AS SITE_CODE ,
                                CCODE AS Company_Code
                       FROM     SYSTEM_PLANT ,
                                SYSTEM_USER_PLANT
                       WHERE    SYSTEM_PLANT.LOCATION = SYSTEM_USER_PLANT.PLANT_LOCATION AND SYSTEM_PLANT.TYPE = SYSTEM_USER_PLANT.PLANT_TYPE ) b ON a.SITE_CODE = b.SITE_CODE
           LEFT JOIN dbo.CHANGE_HISTORY c
           ON  a.CONTRACT_M_UID=c.OBJ_UID
           LEFT JOIN dbo.SYSTEM_USERS d
           ON a.APPLICANT=d.ACCOUNT 
           WHERE    a.CONTRACT_TYPE <> '' AND CONTRACT_NO NOT IN ( '09361602031', '09361604008', '09361604028', '09361604030', 'GPB1512002', 'HUA-2015-0393-IE' )  
           AND  c.NT_ACCOUNT IS NOT NULL
           AND d.ACCOUNT  IS NOT NULL
";



            List<Insert_Contract_M_1> spp_contract_m = new List<Insert_Contract_M_1>();
            List<SPP.Econtract2._0.Contract_M> Contract_M_list = new List<SPP.Econtract2._0.Contract_M>();
            using (var context = new SPP_ProductionEntities())
            {
                spp_contract_m = context.Database.SqlQuery<Insert_Contract_M_1>(sql).ToList();
                foreach (var item in spp_contract_m)
                {
                    //try
                    //{
                    //    item.Applicant_UID = users.Where(m => m.User_NTID.ToLower() == item.APPLICANT.ToLower()).FirstOrDefault().Users_UID.ToString();
                    //}
                    //catch
                    //{
                    //    //item.Applicant_UID = modified_guid.ToString();
                    //    item.Applicant_UID = string.Empty;
                    //}


                    item.Applicant_UID = users.Where(m => m.User_NTID.ToLower() == item.APPLICANT.ToLower()).FirstOrDefault().Users_UID.ToString();
                    if (item.CONTRACT_TYPE == "Service-Employment/Head Hunter/Training/Internship Agreement")
                    {
                        item.CONTRACT_TYPE = "Employment/Head Hunter/Training/Internship Agreement";
                    }

                    item.JabilEntity_UID = company.Where(m => m.Company_Code == item.Company_Code).FirstOrDefault().Company_UID.ToString();
                    item.ContractType_D_UID = contract_d.Where(m => m.ContractType_D_Name_EN == item.CONTRACT_TYPE).FirstOrDefault().ContractType_D_UID.ToString();


                    item.Department_UID = department.Where(m => m.Company_UID == new Guid(item.JabilEntity_UID) & m.Department_Name == item.DEPARTMENT).FirstOrDefault().Department_UID.ToString();
                    if (item.IS_RENEW.HasValue)
                    {
                        if (Convert.ToInt32(item.IS_RENEW) == 1 || Convert.ToInt32(item.IS_RENEW) == 0)
                        {
                            item.Is_Renew_UID = typecode.Where(m => m.TypeCode_L2_Name == "New Contract").FirstOrDefault().TypeCode_L2_UID.ToString();
                        }
                        if (Convert.ToInt32(item.IS_RENEW) == 2)
                        {
                            item.Is_Renew_UID = typecode.Where(m => m.TypeCode_L2_Name == "Renew with same contract").FirstOrDefault().TypeCode_L2_UID.ToString();
                        }
                        if (Convert.ToInt32(item.IS_RENEW) == 3)
                        {
                            item.Is_Renew_UID = typecode.Where(m => m.TypeCode_L2_Name == "Renew with differ contract").FirstOrDefault().TypeCode_L2_UID.ToString();
                        }
                    }
                    else
                    {
                        item.Is_Renew_UID = typecode.Where(m => m.TypeCode_L2_Name == "New Contract").FirstOrDefault().TypeCode_L2_UID.ToString();
                    }


                    Contract_M model_Contract_M = new Contract_M();
                    model_Contract_M.Contract_M_UID = new Guid(item.CONTRACT_M_UID);
                    model_Contract_M.Contract_No = item.CONTRACT_NO;
                    model_Contract_M.Applicant_UID = new Guid(item.Applicant_UID);
                    model_Contract_M.JabilEntity_UID = new Guid(item.JabilEntity_UID);
                    model_Contract_M.Department_UID = new Guid(item.Department_UID);
                    model_Contract_M.ContractType_D_UID = new Guid(item.ContractType_D_UID);
                    if (item.IS_TEMPLATE_USED.HasValue)
                    {
                        model_Contract_M.Is_Template_Used = Convert.ToBoolean(item.IS_TEMPLATE_USED);
                    }
                    else
                    {
                        model_Contract_M.Is_Template_Used = false;
                    }

                    model_Contract_M.Contract_CostCenter = String.Empty;
                    model_Contract_M.Contract_Name = item.CONTRACT_NAME;
                    model_Contract_M.Contract_Desc = item.CONTRACT_DESC;
                    model_Contract_M.Vendor_Code = String.Empty;
                    model_Contract_M.Customer_Code = String.Empty;
                    model_Contract_M.Contractor = item.CONTRACTOR;
                    model_Contract_M.Is_Renew_UID = new Guid(item.Is_Renew_UID);
                    model_Contract_M.Previous_Contract_No = String.Empty;
                    model_Contract_M.Copies = item.CONTRACT_COPIES;
                    if (item.CONTRACT_VALUE != 0)
                    {
                        model_Contract_M.With_Payment = true;
                    }
                    else
                    {
                        model_Contract_M.With_Payment = false;

                    }

                    model_Contract_M.Currency = item.CURRENCY_CODE;
                    model_Contract_M.Payment_Amount = item.CONTRACT_VALUE;
                    model_Contract_M.With_VAT = null;
                    model_Contract_M.Amount_Involve_Per_Year = null;
                    model_Contract_M.Amount_Involve_perYear_USD = null;
                    model_Contract_M.Exchange_Rate_toUSD = null;
                    model_Contract_M.Payment_Schedule = null;
                    model_Contract_M.Begin_Date = null;
                    model_Contract_M.End_Date = null;
                    model_Contract_M.Expiration_Notice_Date = item.EXPIRATION_NOTICE_DATE;
                    model_Contract_M.Delivery_Date = item.DELIVERY_DATE;
                    model_Contract_M.Project_Commencement_Date = item.PRO_COMPLETION_DATE;
                    model_Contract_M.Project_Completion_Date = item.PRO_COMPLETION_DATE;
                    model_Contract_M.Warranty_Begin_Date = item.WARRANTY_PERIOD;
                    model_Contract_M.Warranty_End_Date = DateTime.Now;
                    model_Contract_M.Estimate_Effective_Date = item.ESTIMATE_EFFECTIVE_DATE;
                    model_Contract_M.Is_MultipleContractorMaster = null;
                    model_Contract_M.Status = item.STATUS;
                    model_Contract_M.Version = item.VERSION;
                    model_Contract_M.Is_Latest = Convert.ToBoolean(item.IS_LATEST);
                    model_Contract_M.Modified_UID = users.Where(m => m.User_NTID.ToLower() == item.Modify_NTID.ToLower()).FirstOrDefault().Users_UID;
                    //try
                    //{
                    //    model_Contract_M.Modified_UID = users.Where(m => m.User_NTID == item.Modify_NTID).FirstOrDefault().Users_UID;
                    //}
                    //catch
                    //{
                    //    //item.Applicant_UID = string.Empty;
                    //    //model_Contract_M.Modified_UID = modified_guid;
                    //    model_Contract_M.Modified_UID = Guid.Empty;
                    //}


                    model_Contract_M.Modified_Date = DateTime.Now;
                    model_Contract_M.Modified_Remarks = modify_remarks;
                    model_Contract_M.CPT_No = String.Empty;
                    model_Contract_M.SRM_No = String.Empty;

                    Contract_M_list.Add(model_Contract_M);

                }

            }


            using (var context = new SPP_MVC_Entities())
            {
                //context.SPP.Econtract2._0.Contract_M.AddRange(Contract_M_list);
                //context.Econtract2._0.CONTRACT_M.AddRange(Contract_M_list);
                context.Contract_M.AddRange(Contract_M_list);
                context.SaveChanges();
            }



        }

        public static void Insert_Tb_Contract_Attachment()
        {

            List<WF_REVIEW_TEAM_CONTRACT_SITE> reviewTeam = new List<WF_REVIEW_TEAM_CONTRACT_SITE>();
            List<CONTRACT_D> contract_d = new List<CONTRACT_D>();
//            var sql = @"
//SELECT a.*,b.ACCOUNT FROM CONTRACT_D a
//LEFT JOIN dbo.SYSTEM_USERS b ON a.CREATOR=b.ACCOUNT 
//WHERE CONTRACT_M_UID IN  (SELECT CONTRACT_M_UID FROM dbo.CONTRACT_M where  CONTRACT_TYPE <> '' AND CONTRACT_NO NOT IN ( '09361602031', '09361604008', '09361604028', '09361604030', 'GPB1512002', 'HUA-2015-0393-IE' ) ) AND b.ACCOUNT IS NOT NULL 
//"
//;

//            var sql = @"
//SELECT  *
//FROM    CONTRACT_D
//WHERE   CONTRACT_M_UID IN ( SELECT  CONTRACT_M_UID
//                            FROM    dbo.CONTRACT_M a LEFT JOIN  dbo.SYSTEM_USERS b ON
//                            a.APPLICANT=b.ACCOUNT
//                            LEFT JOIN  dbo.CHANGE_HISTORY c
//                            ON a.APPLICANT=c.NT_ACCOUNT
//                            WHERE   CONTRACT_TYPE <> '' AND CONTRACT_NO NOT IN ( '09361602031', '09361604008', '09361604028', '09361604030', 'GPB1512002', 'HUA-2015-0393-IE' )
//                            AND b.ACCOUNT IS NOT NULL
//                            AND d.NT_ACCOUNT IS NOT NULL
//                             )
//";

            var sql = @"
SELECT  *
FROM    CONTRACT_D
WHERE   CONTRACT_M_UID IN ( SELECT DISTINCT CONTRACT_M_UID
                            FROM    dbo.CONTRACT_M a INNER JOIN  dbo.SYSTEM_USERS b ON
                            a.APPLICANT=b.ACCOUNT
                      
                            WHERE   CONTRACT_TYPE <> '' AND CONTRACT_NO NOT IN ( '09361602031', '09361604008', '09361604028', '09361604030', 'GPB1512002', 'HUA-2015-0393-IE' )
                            AND b.ACCOUNT IS NOT NULL
                           
                             )
";

            List<Guid> contract_M_uid_str = new List<Guid>();
            using (var context = new SPP_MVC_Entities())
            {
                contract_M_uid_str= context.Contract_M.Select(m => m.Contract_M_UID).ToList<Guid>();

            }

           

            using (var context = new SPP_ProductionEntities())
            {
                reviewTeam = context.WF_REVIEW_TEAM_CONTRACT_SITE.ToList();
                contract_d = context.Database.SqlQuery<CONTRACT_D>(sql).ToList();

                contract_d = contract_d.Where(m => contract_M_uid_str.Contains(new Guid(m.CONTRACT_M_UID))).ToList();

            }

            List<Users> users = new List<Users>();
            List<Contract_Attachment> attachment_list = new List<Contract_Attachment>();
            using (var context = new SPP_MVC_Entities())
            {
                users = context.Users.ToList();

                foreach (var item in contract_d)
                {
                    Contract_Attachment model_Contract_Attachment = new Contract_Attachment();
                    model_Contract_Attachment.Contract_Attachment_UID = new Guid(item.CONTRACT_D_UID);
                    model_Contract_Attachment.Contract_M_UID = new Guid(item.CONTRACT_M_UID);
                    model_Contract_Attachment.Attachment_Type = item.TEMPLATE_TYPE;
                    model_Contract_Attachment.System_File_Name = item.FILE_NAME;
                    model_Contract_Attachment.Original_File_Name = item.FILE_NAME; ;
                    model_Contract_Attachment.Display_File_Name = item.FILE_NAME; ;
                    model_Contract_Attachment.File_Size = 0;
                    model_Contract_Attachment.File_Path = String.Empty;
                    model_Contract_Attachment.Tempkey = Guid.Empty;
                    model_Contract_Attachment.Uploaded_UID =users.Where(m=>m.User_NTID.ToLower()==item.CREATOR.ToLower()).FirstOrDefault().Users_UID;
                    model_Contract_Attachment.Uploaded_Date =Convert.ToDateTime(item.UPLOAD_DATE);
                    attachment_list.Add(model_Contract_Attachment);
         
                }

                context.Contract_Attachment.AddRange(attachment_list);
                context.SaveChanges();
            }

        }

        private static string ConvertToNtids(string ntids, List<Users> users)
        {
            string re = string.Empty;
            if (!string.IsNullOrEmpty(ntids))
            {
                var array = ntids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                for (int i = 0; i < array.Count; i++)
                {

                    if (i == array.Count - 1)
                    {
                        re = re + users.Where(m => m.User_NTID.ToLower() == array[i].ToString().Trim().ToLower()).FirstOrDefault().Users_UID.ToString();
                    }
                    else
                    {
                        re = re + users.Where(m => m.User_NTID.ToLower() == array[i].ToString().Trim().ToLower()).FirstOrDefault().Users_UID.ToString() + ",";

                    }
                }
            }


            return re;
        }

        public static void Insert_Tb_Contract_WfTeam()
        {

            List<Insert_Tb_Contract_WfTeam_1> wf_review_teams = new List<Insert_Tb_Contract_WfTeam_1>();
            using (var context = new SPP_ProductionEntities())
            {
                var sql = @"
SELECT b.CCODE,'' AS Contract_WfTeam_UID,'' AS Department_UID ,'' AS Submit_UID, [SITE_CODE], [DEPARTMENT], [SUBMIT], [FM1], [FM2], [FINANCE], [LEGAL], [FINANCE2], [PUR], [OPM], [OPA], [SCM], [DCC], [1ST_LEGAL_CUSTOMER] AS C1ST_LEGAL_CUSTOMER, [2ND_LEGAL_CUSTOMER] AS C2ND_LEGAL_CUSTOMER, [1ST_LEGAL_SERVICE] AS C1ST_LEGAL_SERVICE, [2ND_LEGAL_SERVICE] AS C2ND_LEGAL_SERVICE, [LEGAL_CUSTOMER_NDA], [COST_CENTER]  FROM dbo.WF_REVIEW_TEAM_CONTRACT_SITE a
LEFT JOIN dbo.SYSTEM_PLANT b ON
a.SITE_CODE=b.NAME_0 
";
                wf_review_teams = context.Database.SqlQuery<Insert_Tb_Contract_WfTeam_1>(sql).ToList();
            }
            List<Users> users = new List<Users>();
            List<Department> department = new List<Department>();
            List<Company> company = new List<Company>();
            List<Contract_WfTeam> contract_wf_team = new List<Contract_WfTeam>();
            using (var context = new SPP_MVC_Entities())
            {
                users = context.Users.ToList();
                department = context.Department.ToList();
                company = context.Company.ToList();


                foreach (var item in wf_review_teams)
                {
                    var company_uid = company.Where(m => m.Company_Code == item.CCODE).FirstOrDefault().Company_UID;

                    item.Department_UID = department.Where(m => m.Department_Name == item.DEPARTMENT & m.Company_UID == company_uid).FirstOrDefault().Department_UID.ToString();

                    item.Submit_UID = users.Where(m => m.User_NTID.ToLower() == item.SUBMIT.ToLower()).FirstOrDefault().Users_UID.ToString();

                    Contract_WfTeam model_Contract_WfTeam = new Contract_WfTeam();
                    model_Contract_WfTeam.Contract_WfTeam_UID = Guid.NewGuid();
                    model_Contract_WfTeam.Department_UID = new Guid(item.Department_UID);
                    model_Contract_WfTeam.Submit_UID = new Guid(item.Submit_UID);
                    model_Contract_WfTeam.FuntionManager1_UIDs = ConvertToNtids(item.FM1, users);
                    model_Contract_WfTeam.FuntionManager2_UIDs = ConvertToNtids(item.FM2, users);
                    model_Contract_WfTeam.FunctionDirector_UIDs = string.Empty;
                    model_Contract_WfTeam.FunctionVP_UIDs = String.Empty;
                    model_Contract_WfTeam.BUManager_UIDs = String.Empty;
                    model_Contract_WfTeam.BUDirector_UIDs = String.Empty;
                    model_Contract_WfTeam.BUVP_UIDs = String.Empty;
                    model_Contract_WfTeam.PurchasingManager_UIDs = ConvertToNtids(item.PUR, users);
                    model_Contract_WfTeam.PurchasingDirector_UIDs = String.Empty;
                    model_Contract_WfTeam.SupplyChainManager_UIDs = ConvertToNtids(item.SCM, users);
                    model_Contract_WfTeam.SupplyChainDirector_UIDs = String.Empty;
                    model_Contract_WfTeam.FinanceManager_UIDs = ConvertToNtids(item.FINANCE, users);
                    model_Contract_WfTeam.FinanceController_UIDs = ConvertToNtids(item.FINANCE2, users);
                    model_Contract_WfTeam.FinanceSeniorController_UIDs = String.Empty;
                    model_Contract_WfTeam.LegalGeneral1_UIDs = String.Empty;
                    model_Contract_WfTeam.LegalGeneral2_UIDs = String.Empty;
                    model_Contract_WfTeam.LegalCustomer_ABC_UIDs = string.Empty;
                    model_Contract_WfTeam.LegalCustomer_NDA_UIDs = ConvertToNtids(item.LEGAL_CUSTOMER_NDA, users);
                    model_Contract_WfTeam.LegalCustomer1_UIDs = ConvertToNtids(item.C1ST_LEGAL_CUSTOMER, users);
                    model_Contract_WfTeam.LegalCustomer2_UIDs = ConvertToNtids(item.C2ND_LEGAL_CUSTOMER, users);
                    model_Contract_WfTeam.LegalService1_UIDs = ConvertToNtids(item.C1ST_LEGAL_SERVICE, users);
                    model_Contract_WfTeam.LegalService2_UIDs = ConvertToNtids(item.C2ND_LEGAL_SERVICE, users);
                    model_Contract_WfTeam.LegalUS1_UIDs = String.Empty;
                    model_Contract_WfTeam.LegalUS2_UIDs = String.Empty;
                    model_Contract_WfTeam.OPManager_UIDs = ConvertToNtids(item.OPM, users);
                    model_Contract_WfTeam.OPDirector_UIDs = String.Empty;
                    model_Contract_WfTeam.OPAssisstant_UIDs = ConvertToNtids(item.OPA, users);
                    model_Contract_WfTeam.Upload_PIC_UIDs = String.Empty;
                    model_Contract_WfTeam.Upload_PICFunctionManager_UIDs = String.Empty;
                    model_Contract_WfTeam.File_In_PIC_UIDs = String.Empty;
                    model_Contract_WfTeam.Is_Enable = true;
                    model_Contract_WfTeam.Modified_UID = modified_guid;
                    model_Contract_WfTeam.Modified_Date = DateTime.Now;
                    model_Contract_WfTeam.Modified_Remarks = modify_remarks;
                    contract_wf_team.Add(model_Contract_WfTeam);
                }
                context.Contract_WfTeam.AddRange(contract_wf_team);
                context.SaveChanges();

            }

           



        }

        public static void Insert_Tb_WfTask()
        {
            List<string> contract_M_no = new List<string>();
            List<Users> users = new List<Users>();
            using (var context = new SPP_MVC_Entities())
            {
                contract_M_no = context.Contract_M.Select(m => m.Contract_No.ToString().ToLower()).Distinct().ToList<string>();
                users = context.Users.ToList();

            }

            List<WF_TASK> wf_old_task = new List<WF_TASK>();
            using (var context = new SPP_ProductionEntities())
            {

                //context.Database.CommandTimeout = 60 * 5;
                //wf_old_task = context.WF_TASK.Where(m => contract_M_uid_str.Contains(m.UID.ToLower()) && string.IsNullOrEmpty(m.STATE)).ToList();

                wf_old_task = context.WF_TASK.Where(m => m.STATE == null).ToList();
                wf_old_task= wf_old_task.Where(m=> contract_M_no.Contains(m.OBJ_NO.ToLower())).ToList();
                //wf_old_task = context.WF_TASK.ToList().Where(m => contract_M_uid_str.Contains(new Guid(m.UID)) & m.STATE != null).ToList();

            }

            using (var context = new SPP_MVC_Entities())
            {
                //context.Database.CommandTimeout = 60 * 5;
                List<WfTask> WfTask_list= new List<WfTask>();

                foreach (var item in wf_old_task)
                {

                    WfTask model_WfTask_History = new WfTask();
                    model_WfTask_History.WfTask_UID = new Guid(item.UID);
                    model_WfTask_History.Module_UID = module_uid;
                    model_WfTask_History.Obj_No = item.OBJ_NO;
                    model_WfTask_History.Task_Name = item.TASK;
                    model_WfTask_History.Task_Role = item.ROLE;
                    model_WfTask_History.Task_Owner = users.Where(m => m.User_NTID.ToLower() == item.OWNER.Trim().ToLower()).FirstOrDefault().Users_UID;
                    model_WfTask_History.State = item.STATE;
                    model_WfTask_History.Comments = item.COMMENTS;
                    model_WfTask_History.Assigned_Date = Convert.ToDateTime(item.ASSIGN_DATETIME);
                    //model_WfTask_History.Completed_Date = Convert.ToDateTime(item.COMPLETE_DATETIME);
                    model_WfTask_History.Return_Times = Convert.ToInt32(item.REVIEW_LOOP);
                    if (!string.IsNullOrEmpty(item.DELEGATE_FROM))
                    {
                        model_WfTask_History.Delegation_From_UID = users.Where(m => m.User_NTID.Trim().ToLower() == item.DELEGATE_FROM.Trim().ToLower()).FirstOrDefault().Users_UID;
                    }
        
        
                    model_WfTask_History.Resubmit_Routing = null;
                    WfTask_list.Add(model_WfTask_History);

                }
                context.WfTask.AddRange(WfTask_list);
                context.SaveChanges();

            }


        }

        public static void Insert_Tb_WfTask_History()
        {
            List<string> contract_M_no = new List<string>();
            List<Users> users = new List<Users>();
            using (var context = new SPP_MVC_Entities())
            {
                contract_M_no = context.Contract_M.Select(m => m.Contract_No.ToString().ToLower()).Distinct().ToList<string>();
                users = context.Users.ToList();

            }

            List<WF_TASK> wf_old_task = new List<WF_TASK>();
            using (var context = new SPP_ProductionEntities())
            {

                //context.Database.CommandTimeout = 3600 * 2;
                wf_old_task = context.WF_TASK.Where(m =>!string.IsNullOrEmpty(m.STATE)).ToList();
                //wf_old_task = context.WF_TASK.Where(m => contract_M_no.Contains(m.OBJ_NO.ToLower())).ToList();
                //wf_old_task = context.WF_TASK.ToList().Where(m => contract_M_uid_str.Contains(new .uid(m.UID)) & m.STATE != null).ToList();

            }

            using (var context = new SPP_MVC_Entities())
            {
                //context.Database.CommandTimeout = 180 * 1000000;
                List<WfTask_History> WfTask_History_list = new List<WfTask_History>();

                foreach (var item in wf_old_task)
                {
                    if (contract_M_no.Contains(item.OBJ_NO))
                    {
                        var user = users.Where(m => m.User_NTID.ToLower().Trim() == item.OWNER.ToLower().Trim()).FirstOrDefault();
                        if (user!=null)
                        {
                            WfTask_History model_WfTask_History = new WfTask_History();
                            model_WfTask_History.WfTask_History_UID = new Guid(item.UID);
                            model_WfTask_History.Module_UID = module_uid;
                            model_WfTask_History.Obj_No = item.OBJ_NO;
                            model_WfTask_History.Task_Name = item.TASK;
                            model_WfTask_History.Task_Role = item.ROLE;
                            model_WfTask_History.Task_Owner = users.Where(m => m.User_NTID.ToLower().Trim() == item.OWNER.ToLower().Trim()).FirstOrDefault().Users_UID;
                            model_WfTask_History.State = item.STATE;
                            if (!string.IsNullOrEmpty(model_WfTask_History.Comments))
                            {
                                if (model_WfTask_History.Comments.Length < 500)
                                {
                                    model_WfTask_History.Comments = item.COMMENTS;
                                }
                                else
                                {

                                    model_WfTask_History.Comments = item.COMMENTS.Substring(0, 500);
                                }
                            }
                           

                            model_WfTask_History.Assigned_Date = Convert.ToDateTime(item.ASSIGN_DATETIME);
                            model_WfTask_History.Completed_Date = Convert.ToDateTime(item.COMPLETE_DATETIME);
                            model_WfTask_History.Return_Times = Convert.ToInt32(item.REVIEW_LOOP);
                            if (item.DELEGATE_FROM != null&&!string.IsNullOrEmpty(item.DELEGATE_FROM))
                            {
                                model_WfTask_History.Delegation_From_UID = users.Where(m => m.User_NTID.ToLower().Trim() == item.DELEGATE_FROM.ToLower().Trim()).FirstOrDefault().Users_UID;
                            }

                            model_WfTask_History.Backup_Date = DateTime.Now;
                            model_WfTask_History.Resubmit_Routing = null;
                            WfTask_History_list.Add(model_WfTask_History);
                        }
                        

                    }


                }
                context.WfTask_History.AddRange(WfTask_History_list);


                try
                {
                    context.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    var Re = string.Empty;
                    foreach (var errors in ex.EntityValidationErrors)
                    {
                        foreach (var item in errors.ValidationErrors)
                        {
                            Re = Re + (item.ErrorMessage + item.PropertyName);
                        }
                    }

                }

            }


        }

        public static void Insert_Tb_WfTaskDelaySetting()
        {

            List<WF_TASK_DELAY_CONFIG> delay_config = new List<WF_TASK_DELAY_CONFIG>();
            using (var context = new SPP_ProductionEntities())
            {
                 delay_config = context.WF_TASK_DELAY_CONFIG.ToList();
            }

            List<WfTaskDelaySetting> delay_setting_list = new List<WfTaskDelaySetting>();

            using (var context = new SPP_MVC_Entities())
            {
                foreach (var item in delay_config)
                {
                    WfTaskDelaySetting model_WfTaskDelaySetting = new WfTaskDelaySetting();
                    model_WfTaskDelaySetting.WfTaskDelaySetting_UID = Guid.NewGuid();
                    model_WfTaskDelaySetting.Module_UID = module_uid;
                    model_WfTaskDelaySetting.Task_Name = String.Empty;
                    model_WfTaskDelaySetting.Task_Role = item.ROLE;
                    model_WfTaskDelaySetting.Delay_Days = Convert.ToInt32(item.DELAY_DAYS);
                    model_WfTaskDelaySetting.Reminder = item.REMINDER;
                    model_WfTaskDelaySetting.Modified_UID = modified_guid;
                    model_WfTaskDelaySetting.Modified_Date = DateTime.Now;
                    model_WfTaskDelaySetting.Modified_Remarks = item.REMARKS;
                    delay_setting_list.Add(model_WfTaskDelaySetting);

                }
                context.WfTaskDelaySetting.AddRange(delay_setting_list);
                context.SaveChanges();
            }

        }

        public static void Insert_Tb_WfDelegation()
        {
            List<CONTRACT_DELEGATION> CONTRACT_DELEGATION_list = new List<CONTRACT_DELEGATION>();
           
            using (var context = new SPP_ProductionEntities())
            {
                CONTRACT_DELEGATION_list = context.CONTRACT_DELEGATION.ToList();

            }

            List<WfDelegation> WfDelegation_list = new List<WfDelegation>();
            List<Users> users = new List<Users>();
            using (var context = new SPP_MVC_Entities())
            {
                users = context.Users.ToList();
                foreach (var item in CONTRACT_DELEGATION_list)
                {
                    WfDelegation model_WfDelegation = new WfDelegation();
                    model_WfDelegation.WfDelegation_UID = Guid.NewGuid();
                    model_WfDelegation.Module_UID = module_uid;
                    model_WfDelegation.Principal_UID = users.Where(m => m.User_NTID.Trim().ToLower()==item.CURRENT_USER.Trim().ToLower()).FirstOrDefault().Users_UID;
                    model_WfDelegation.Deputy_UID = users.Where(m => m.User_NTID.Trim().ToLower() == item.DEPUTY_USER.Trim().ToLower()).FirstOrDefault().Users_UID;
                    model_WfDelegation.Begin_Date =item.START_DATE;
                    model_WfDelegation.End_Date = item.END_DATE;
                    model_WfDelegation.Modified_UID =modified_guid;
                    model_WfDelegation.Modified_Date = DateTime.Now;
                    model_WfDelegation.Modified_Remarks = item.REMARK;
                    WfDelegation_list.Add(model_WfDelegation);

                }
                //WfDelegation_list.AddRange(model_WfDelegation);
                context.WfDelegation.AddRange(WfDelegation_list);
                context.SaveChanges();
            }

        }

        public static void Insert_Tb_WfDelegation_History()
        {
            List<CONTRACT_DELEGATION_BACKUP> CONTRACT_DELEGATION_list = new List<CONTRACT_DELEGATION_BACKUP>();

            using (var context = new SPP_ProductionEntities())
            {
                CONTRACT_DELEGATION_list = context.CONTRACT_DELEGATION_BACKUP.ToList();

            }

            List<WfDelegation_History> WfDelegation_list = new List<WfDelegation_History>();
            List<Users> users = new List<Users>();
            using (var context = new SPP_MVC_Entities())
            {
                users = context.Users.ToList();
                foreach (var item in CONTRACT_DELEGATION_list)
                {
                    WfDelegation_History model_WfDelegation = new WfDelegation_History();
                    model_WfDelegation.WfDelegation_History_UID = Guid.NewGuid();
                    model_WfDelegation.Module_UID = module_uid;
                    model_WfDelegation.Principal_UID = users.Where(m => m.User_NTID.Trim().ToLower() == item.CURRENT_USER.Trim().ToLower()).FirstOrDefault().Users_UID;
                    model_WfDelegation.Deputy_UID = users.Where(m => m.User_NTID.Trim().ToLower() == item.DEPUTY_USER.Trim().ToLower()).FirstOrDefault().Users_UID;
                    model_WfDelegation.Begin_Date = item.START_DATE;
                    model_WfDelegation.End_Date = item.END_DATE;
                    model_WfDelegation.Modified_UID = modified_guid;
                    model_WfDelegation.Modified_Date = DateTime.Now;
                    model_WfDelegation.Modified_Remarks = item.REMARK;
                    WfDelegation_list.Add(model_WfDelegation);

                }
                //WfDelegation_list.AddRange(model_WfDelegation);
                context.WfDelegation_History.AddRange(WfDelegation_list);
                context.SaveChanges();
            }


        }

        public static void Insert_Tb_WfEmail_StopExpirationNotice()
        {

            List<CONTRACT_EXPIRATION_NOTICE> CONTRACT_EXPIRATION_NOTICE_list = new List<CONTRACT_EXPIRATION_NOTICE>();
            using (var context = new SPP_ProductionEntities())
            {
                CONTRACT_EXPIRATION_NOTICE_list = context.CONTRACT_EXPIRATION_NOTICE.ToList();

            }

            List<WfEmail_StopExpirationNotice> WfEmail_StopExpirationNotice_list = new List<WfEmail_StopExpirationNotice>();
            using (var context = new SPP_MVC_Entities())
            {

                foreach (var item in CONTRACT_EXPIRATION_NOTICE_list)
                {
                    WfEmail_StopExpirationNotice model_WfEmail_StopExpirationNotice = new WfEmail_StopExpirationNotice();
                    model_WfEmail_StopExpirationNotice.WfEmail_StopExpirationNotice_UID = Guid.NewGuid();
                    model_WfEmail_StopExpirationNotice.Obj_Table ="Contract_M";
                    model_WfEmail_StopExpirationNotice.Obj_No =item.CONTRACT_NO;
                    model_WfEmail_StopExpirationNotice.Is_Renew = Convert.ToBoolean(item.IS_RENEW);
                    model_WfEmail_StopExpirationNotice.Modified_UID =modified_guid;
                    model_WfEmail_StopExpirationNotice.Modified_Date = DateTime.Now;
                    model_WfEmail_StopExpirationNotice.Modified_Remarks = String.Empty;
                    WfEmail_StopExpirationNotice_list.Add(model_WfEmail_StopExpirationNotice);

                }
                context.WfEmail_StopExpirationNotice.AddRange(WfEmail_StopExpirationNotice_list);
                context.SaveChanges();
            }
            


        }

        public static void test()
        {
            WF_TASK wf_task = new WF_TASK();

            SPP_Produciton_BaseRepository<WF_TASK> w = new SPP_Produciton_BaseRepository<WF_TASK>();
            w.LoadEntities(m=>m.OBJ_NO!=null).FirstOrDefault();
            //w.AddEntities();

        }

       
    }

    //SITE_CODE DEPARTMENT  Company_Code Department_UID, Company_UID

}
