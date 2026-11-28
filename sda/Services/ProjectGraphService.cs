using sda.DTOs;

namespace sda.Services
{
    public class ProjectGraphService
    {
        private readonly ProjectService _project;
        private readonly PackageService _package;
        public ProjectGraphService(ProjectService project, PackageService package)
        {
            _project = project;
            _package = package;
        }
        public async Task BuildGraph(CreateProjectDto dto, string json)
        {
            var project = await _project.CreateProject(dto);

            var packages = PackageLockParser.TryParse(json);

            var directPackages = packages[""].Dependencies;

            if(directPackages is null || directPackages.Count == 0)
                return;

            foreach(var dp in directPackages!)
            {
                await _package.CreateDirectPackage(new CreateDirectPackageDto
                {
                    projectName = project.name,
                    packageName = dp.Key,
                    ecosystem = "npm",
                    description = dp.Value
                });
            }
        }
    }
}