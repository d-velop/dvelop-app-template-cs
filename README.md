# cs-app-template

This template contains everything you need to write an app for the d.velop cloud.

To demonstrate all the aspects of app development the hypothetical but not trivial use case
of *an employee applying for vacation* is implemented.

## Getting Started

Just clone this repo and follow the [build instructions](#build) to get the sample app up and running.
After this adjust the code to fit the purpose of your own business problem/app.

### Prerequisites

A linux docker container is used for the build and deployment process of the app.
So besides docker the only thing you need on your local development system is a 
git client and an editor or IDE for C#.

To develop a d.velop cloud app you will need to install [.NET Core SDK 2.1](https://dotnet.microsoft.com/download/dotnet-core/2.1). Newer versions of the SDK will also work, but keep in mind, that AWS Lambda will only [support LTS versions](https://github.com/aws/aws-lambda-dotnet) of `.NET Core`. 

If you use Microsoft Visual Studio you will need "ASP.NET and web development", ".NET desktop development" and ".NET Core cross-platform development" workloads installed.

### Build

Build the image for your app's build container ([Dockerfile](./buildcontainer/Dockerfile)).

Adjust the `APPNAME` and if needet the `BUILDCONTAINER` vars in [docker-build.bat](docker-build.bat) **and** [docker-build.sh](docker-build.sh) to match
the name of your app. And execute the build with

```
docker-build build
```

This will build a self contained web application `dist/windows_<rev>.zip` for windows, which can be used to run and test your app as a local process on your
dev system and a deployment packages for aws lambda `dist/lambda_code_<rev>.zip` which should be used for the production deployment of your app in d.velop cloud.

## Run and test your app locally

Just extract the newest `dist/windows_<rev>.zip` and run the included `HostApplication.exe` to run and test your app on a local environment.
Please keep in mind, that some functions like authentication
which require the presence of additional apps (e.g. IdentityProviderApp), won't work because these apps are not available on your local system.

If you build and run the application from an IDE, keep in mind, that there may be some issues, if the IDE and the build-container are using the same folders.

## Rename the app

You should change the name of the app so that it reflects the business problem you would like to solve.

Each appname in d.velop cloud must be unique. To facilitate this every provider/company chooses
a unique provider prefix which serves as a namespace for the apps of this provider.
The prefix can be selected during the registration process in d.velop cloud.
If you choose a provider prefix which corresponds to your company name or an abbreviation of the company name
it's very likely that it is available when you later register your app in d.velop cloud.

For example if your company is named *Super Duper Software Limited* and the domain of your app 
is *employees applying for vacation* your app should be named
something like `superduperltd-vacationprocess`App. Note that the `App` suffix isn't used in the configuration files. 

Apps belonging to the core d.velop cloud platform don't have a provider prefix. 

For now the following places have to be adjusted manually as soon as the name of the app changes:

1.  `Makefile` change the `APP_NAME` variable. Furthermore change the `DOMAIN_SUFFIX` to a domain you own like `yourcompany.com`
2.  `/terraform/backend.tf` change the `bucket` names (2 occurrences)
3.  `docker-build.bat` and `dockerbuild.sh` change the `APPNAME` variable      
4.  `/Remote/Startup.cs` change the Default value for the `Configuration["APP_NAME"]`
    
The 'Replace' function of your IDE should help.

**Please finish at least step 1 and step 2 before you [deploy](#deployment) your app because the names of a lot of
AWS resources are derived from the `APP_NAME` and `DOMAIN_SUFFIX`. Changing them afterwards requires a
redeployment of the AWS resources which takes some time.**


## Deployment

Configure your AWS credentials by using one of the methods described in
[Configuring the AWS CLI](https://docs.aws.amazon.com/cli/latest/userguide/cli-chap-getting-started.html).
For example set the `AWS_ACCESS_KEY_ID` and `AWS_SECRET_ACCESS_KEY` environment variables.


**Windows**

```
SET AWS_ACCESS_KEY_ID=<YOUR-ACCESS-KEY-ID>
SET AWS_SECRET_ACCESS_KEY=<YOUR-SECRET-ACCESS-KEY>
```

**Linux**

```
export AWS_ACCESS_KEY_ID=<YOUR-ACCESS-KEY-ID>
export AWS_SECRET_ACCESS_KEY=<YOUR-SECRET-ACCESS-KEY>
```

Deploy the lambda function and all other AWS Ressources like AWS API Gateway.

```
docker-build deploy
```

The build container uses [Terraform](https://www.terraform.io/) to manage the AWS ressources and to deploy
your lambda function. This tool implements a desired state mechanism which means the first execution will take some time
to provision all the required AWS ressources. Consecutive executions will only deploy the difference between the desired state
(e.g. the new version of your lambda function) and the state which is already deployed (other AWS ressources which won't change
between deployments) and will be much quicker.

### Test your endpoint

The endpoint URLs are logged at the end of the deployment. Just invoke them in a browser to test your app.  
 
```
Apply complete! Resources: 0 added, 0 changed, 0 destroyed.

Outputs:

endpoint = [
    https://xxxxxxxxxx.execute-api.eu-central-1.amazonaws.com/prod/vacationprocess/,
    https://xxxxxxxxxx.execute-api.eu-central-1.amazonaws.com/dev/vacationprocess/
]

```

To watch the current deployment state you can invoke

```
docker-build show 
```

at any time without changing your deployment.

### Deployment of a new app version

Just follow the [deployment](#deployment) steps. A new deployment package for the lambda function will be build automatically.

### Additional AWS resources

The terraform deployment configuration contains 2 additonal modules which are disabled by default.
Just uncomment the corresponding lines in `/terraform/main.tf` to use them but **ensure that the DNS resolution
for your hosted zone works before you use these modules**. Read the comments in the terraform file.

#### asset_cdn
This module uses *aws cloudfront* as a CDN for your static assets. Furthermore it allows you to define
a custom domain for your assets instead of the s3 URL. Your deployment should work perfectly without this module.

#### api_custom_domain 
This module allows you to define a custom domain for your app endpoints. A custom domain name is required
as soon as you register your app in the d.velop cloud center because the base path of your app must
begin with the name of your app. So instead of the default endpoints

```
    https://xxxxxxxxxx.execute-api.eu-central-1.amazonaws.com/prod/vacationprocess/
    https://xxxxxxxxxx.execute-api.eu-central-1.amazonaws.com/dev/vacationprocess/
```
which base paths begin with `/prod` or `/dev` you need endpoints like

```
    https://vacationprocess.xyzdomain.tld/vactionprocess
    https://dev.vacationprocess.xyzdomain.tld/vactionprocess
```
which are provided by this module.

## Projectstructure

This project is built with a simplified hexagonal architecture pattern in mind.

Some important key aspects are:
- The domain logic **must not** have any knowledge about the hosting environment
- All external systems and ressources (like databases for instance) are `hidden` behind an interface, which are defined by the `domain layer`.
- For every external system there is an adapter, which implements the domain-spefic interface and 'glues' it to the concrete implementation.

**Example:**

The domain logic needs to add, update and list vacations. The interface is defined in [Domain/Vacation/IVacationRepository.cs](Domain/Vacation/IVacationRepository.cs) as

```
public interface IVacationRepository
{
    Guid AddVacation(VacationModel vacation);
    bool UpdateVacation(VacationModel vacation);
    IEnumerable<VacationModel> Vacations { get; }
}
```

There are two *different* implementations for this interface. One for AWS-lambda
([AwsLambda/Adapter/AwsVacationRepository.cs](AwsLambda/Adapter/AwsVacationRepository.cs)) and another for the self hosted environment ([SelfHosted/Adapter/SelfHostedVacationRepository.cs](SelfHosted/Adapter/SelfHostedVacationRepository.cs)).


### Projects

#### `AwsLambda/EntryPoint`

Contains the EntryPoint for AWS-Lambda. This project will be used to bootstrap your AWS-Lambda application. 

You can add AWS-lambda specific ASP.NET Core settings in [LambdaEntryPoint.cs](AwsLambda/EntryPoint/LambdaEntryPoint.cs)

#### `SelfHosted/HostApplication`

Contains the EntryPoint for AWS-Lambda. This project will be used to start your App locally.

You can specific ASP.NET Core settings in [Program.cs](SelfHosted/HostApplication/Program.cs)

#### `Remote`

Contains Controller, Views, static assets and Web-Api specific code.

*Folderstructure*

The folder `Constraints` contains a Constraint to enable Content Negotiation for ASP.NET Core 2.1. For more information about this topic visit [AspNetCore at Github](https://github.com/aspnet/AspNetCore/issues/3891)

All WebApi-Controller are stored within `Controller`. The DTOs and ViewModel-classes are also stored in this folder.

`Formatter` contains a pre-configured Input- and Outputformatter for `application/hal+json`. For more information about hypermedia visit the [IETF Draft](https://tools.ietf.org/html/draft-kelly-json-hal-08)

`Pages` contains an example for a Razor-Page.

`Views` contains MVC-Views. The folder structure within the `Views` folder should be identical to the structure within the `Controller` folder.

`wwwroot` contains static assets. If you want to include some frontendcode like an Angular-Single-page App for instance, you will need to configure the frontend buildprocess to output the assets into this directory. All files in this folder (expect `*.html`) will be copied into the S3 bucket for your assets (See: [Makefile: deploy-assets](Makefile#L62)). The deployment process `docker-build deploy` will create a hash over all files in this directory and create a S3 prefix to enable unlimitted caching.

#### `Plugins`

In this Folder are several projects to implement the interfaces defined by the `Domain` project.

You can separate different implementation for AWS lambda and a self-hosted environment by creating more than one project. 

Example: 
AWS lambda uses a [DynamoDb (Fake)](Plugins/DynamoDbFake/DynamoDbBusinessValueRepository.cs) to implement [IBusinessValueRepository.cs](Domain/Repositories/IBusinessValueRepository.cs). 

The self hosted environment has no persistence and uses a [InMemoryDb](Plugins/InMemoryDb/InMemoryBusinessValueRepository.cs) for testing.

#### `Domain`

Contains your Domain Secifc Logic and should have **no** dependencies to any environment specific library or code to ensure a high testability.

### `buildcontainer`

Contains the `Dockerfile`for the buildenvironment. It is kept in a seperate directory to keep the buildcontext small so that the image can be build as fast as possible.

### `terraform`

Contains the [terraform](https://www.terraform.io/) files.

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests to us.

## License

Please read [LICENSE](LICENSE) for licensing information.

