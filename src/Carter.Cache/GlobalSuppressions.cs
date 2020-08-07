// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Design", "RCS1090:Call 'ConfigureAwait(false)'.", Justification = "Not needed on netcore", Scope = "member", Target = "~M:Carter.Cache.CarterCachingMiddleware.Invoke(Microsoft.AspNetCore.Http.HttpContext)~System.Threading.Tasks.Task")]
[assembly: SuppressMessage("Design", "RCS1090:Call 'ConfigureAwait(false)'.", Justification = "Not neeeded on netcore", Scope = "member", Target = "~M:Carter.Cache.CachedResponse.MapToContext(Microsoft.AspNetCore.Http.HttpContext)~System.Threading.Tasks.Task")]
[assembly: SuppressMessage("Design", "RCS1090:Call 'ConfigureAwait(false)'.", Justification = "Not needed on netcore", Scope = "member", Target = "~M:Carter.Cache.CarterCachingMiddleware.SetCache(Microsoft.AspNetCore.Http.HttpContext,Microsoft.AspNetCore.Http.RequestDelegate,Carter.Cache.CachingOptions)~System.Threading.Tasks.Task")]
