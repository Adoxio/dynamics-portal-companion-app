# Dynamics Portal Buddy

Portal Buddy is a starter template for a companion web app for Dynamics 365 portals.  This template includes an authenticated experience persisting the portal user context across requests between the Dynamics 365 portal and the Portal Buddy web app using Azure AD B2C.

Portal Buddy is built on ASP.NET Core 1.1 with the .NET Framework 4.6.1.  It includes the Dynamics 365 SDK and the Dynamics 365 Xrm Tooling utlizing the CrmServiceClient implemented for ASP.NET Core as CrmCoreServiceClient.

## Objective

Portal Buddy provides a way to extend the Dynamics 365 portal with custom code in a secure manner.  This is accomplished by abstracting the Dynamics 365 portal authentication to a Single Sign On or Secure Token Service so that the user identity can be shared across applications.  This template implements the now natively Dynamics 365 portal supported Azure AD B2C.  Azure AD B2C has also been announced to replace all authentication for the Dynamics 365 portal in the near future.

Utilizing JavaScript front-end frameworks like [angular.js](https://angular.io/), [react.js](https://facebook.github.io/react/), and [vue.js](https://vuejs.org/) you can create seamless user experiences with this template that utilize custom server side code with the .NET Framework and Dynamics 365 SDK.

## Components

#### PortalBuddyWebApp
Starter Template based on ASP.NET Core 1.1 with .NET Framework 4.6.1 Web Application

#### solutions
Starter solutions by Portal Buddy Team:
* Portal Buddy Base - includes default security role for use with S2S user or Connection String user to allow sample code to execute.

#### liquid
Liquid Templates to be added to Dynamics 365 portals configuration for easy component installation

## Building

To build the project, ensure that you have [Git](https://git-scm.com/downloads) installed to obtain the source code, and [Visual Studio 2017](https://docs.microsoft.com/en-us/visualstudio/welcome-to-visual-studio) with [ASP.NET Core 2.0](https://www.microsoft.com/net/download/core) installed to compile the source code.

- Clone the repository using Git:
  ```sh
  git clone https://github.com/Adoxio/dynamics-portal-buddy.git
  ```
- Open the `PortalBuddyWebApp.sln` solution file in Visual Studio

## License

This project uses the [MIT license](https://opensource.org/licenses/MIT).

## Contributions

This project accepts community contributions through GitHub, following the [inbound=outbound](https://opensource.guide/legal/#does-my-project-need-an-additional-contributor-agreement) model as described in the [GitHub Terms of Service](https://help.github.com/articles/github-terms-of-service/#6-contributions-under-repository-license):
> Whenever you make a contribution to a repository containing notice of a license, you license your contribution under the same terms, and you agree that you have the right to license your contribution under those terms.

Please submit one pull request per issue so that we can easily identify and review the changes.