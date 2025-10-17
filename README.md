# 프로젝트 이름
API 만들기 First STEP

## 설명
초보자를 위한 간단하게 API 만들기

## 요구 사항
- .NET 10

## 시작하기
1. 콘솔 프로젝트 생성 

2. csproj 파일을 아래와 같이 수정  자동으로 nuget 설치
```
  	<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Serilog.AspNetCore" Version="9.0.0" />
    <PackageReference Include="Serilog.Settings.Configuration" Version="9.0.0" />
    <PackageReference Include="Serilog.Sinks.Console" Version="6.0.0" />
    <PackageReference Include="Serilog.Sinks.File" Version="7.0.0" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="9.0.6" />
    <PackageReference Include="Swashbuckle.AspNetCore.Swagger" Version="9.0.6" />
  </ItemGroup> 
</Project>
```
3. API 본체 분리 
ApiBody.cs

4. Program.cs에 swagger, serilog 설정 추가
