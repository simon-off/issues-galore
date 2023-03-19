using IssuesGalore.Services;

var ui = new UIService();

await ui.Initialize();
await ui.TicketsOverview();
