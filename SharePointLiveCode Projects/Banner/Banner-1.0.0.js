
function InizializeBanner(jsonData) {

	var divContainer = document.getElementById(jsonData.ContainerDivId);
	divContainer.style.display = "block";
	divContainer.style.backgroundColor = "#f0f0f0";

	var btn1 = document.createElement("div");
	btn1.style.backgroundColor = "Yellow";
	btn1.innerHTML = JSON.stringify(jsonData);
	divContainer.appendChild(btn1);

	//divContainer.innerHTML = JSON.stringify(jsonData)

	jQuery(document).ready(function () {
		getItems(divContainer, jsonData.ListName, jsonData.ListTitle, jsonData.BannerName);
	});

}

function createAllItemsInFolderQuery(folderUrl) {
	var qry = new SP.CamlQuery;
	var viewXml = "<View Scope=\"RecursiveAll\"> " +
                    "<Query>" +
                    "<Where>" +
                                "<Eq>" +
                                    "<FieldRef Name=\"FileDirRef\" />" +
                                    "<Value Type=\"Text\">" + folderUrl + "</Value>" +
                                 "</Eq>" +
                    "</Where>" +
                    "</Query>" +
                    "</View>";

	qry.set_viewXml(viewXml);
	return qry;
};

function getItems(divContainer, listName, listTitle, bannerName) {

	var clientContext = new SP.ClientContext.get_current();
	var website = clientContext.get_web();
	var list = website.get_lists().getByTitle(listTitle);

	var query = createAllItemsInFolderQuery("/Lists/" + listName + '/' + bannerName);

	var allItems = list.getItems(query);

	clientContext.load(allItems);

	clientContext.executeQueryAsync(
        Function.createDelegate(this, function () { onQuerySucceeded(divContainer, allItems); }),
        Function.createDelegate(this, function () { onQueryFailed(); })
    );
}

function onQuerySucceeded(divContainer, allItems) {

	var listItemEnumerator = allItems.getEnumerator();

	while (listItemEnumerator.moveNext()) {

		var listItem = listItemEnumerator.get_current();

		//var news = new News();
		//news.inizialize(divContainer);
		//news.setItem(listItem.get_item('Title'));

		var btn = document.createElement("div");
		btn.style.backgroundColor = "lime";
		btn.innerHTML = listItem.get_item('Title');
		divContainer.appendChild(btn);
	}
}

function onQueryFailed(sender, args) {
	console.log(args.get_stackTrace());
}