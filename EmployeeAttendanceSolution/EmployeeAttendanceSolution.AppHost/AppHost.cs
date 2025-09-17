var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.AttendanceSystem_Auth_API>("attendancesystem-auth-api");

builder.AddProject<Projects.AttendanceSystem_ImportFile_API>("attendancesystem-importfile-api");

builder.Build().Run();
