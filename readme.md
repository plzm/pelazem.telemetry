# Telemetry facade and sinks

pelazem.telemetry is a telemetry facade. The constructor takes either a single ITelemetry sink (pelazem.telemetry.appInsights, pelazem.telemetry.log4net), or a list.

When a telemetry method (Track...) is invoked on the facade, the sink(s) is/are invoked asynchronously and in parallel.

Sinks can be directly instantiated.

pelazem.telemetry.AppInsights wraps the Microsoft Application Insights SDK. Various Track... methods are supported. Parameters not directly supported on the App Insights telemetry info objects are sent as key/value pairs in the telemetry info's Properties Dictionary.

pelazem.telemetry.log4net wraps Apache log4net. Configuration is either programmatic (constructor accepts various config values) or by passing in an XmlElement from a typical XML config file.

Implemented as .NET Standard 2.0 DLLs. For compatibility info, see https://docs.microsoft.com/dotnet/standard/net-standard

---

# PLEASE NOTE STANDARD DISCLAIMER FOR THE ENTIRETY OF THIS REPOSITORY AND ALL ASSETS
## 1. No warranties or guarantees are made or implied.
## 2. All assets here are provided by me "as is". Use at your own risk.
## 3. I am not representing my employer with any files, code, or other assets here, and my employer assumes no liability whatsoever for any use of these files, code, or assets.
## 4. DO NOT USE ANY ASSET HERE IN A PRODUCTION ENVIRONMENT WITHOUT APPROPRIATE REVIEWS, TESTS, and APPROVALS IN YOUR ENVIRONMENT.
