var dynamic;
var table = {
    id: "",
    dtNewRequest: "#dtNewRequest",
    dtAccRequest: "#dtAcceptedRequest",
    dtRejRequest: "#dtRejectedRequest",

    ViewClickAddress: "ManageAdvertisement/ViewRequest?id=",
    DeleteClickAddress: "ManageAdvertisement/RemoveRequest",
    SetIsActive: "ManageAdvertisement/ManageIsActive"
};
var custom = {
    "sLengthMenu": "Display _MENU_ records per page",
    "sZeroRecords": "Nothing found - sorry",
    "sInfo": "Showing _START_ to _END_ of _TOTAL_ records" + "Changes",
    "sInfoEmpty": "Showing 0 to 0 of 0 records",
    "sInfoFiltered": "(filtered from _MAX_ total records)"
};

function ViewRequest(id) {
    $("#modalContent").load(`/Distributer/ManageAdvertisement/ViewRequest/${id}`, function () {
      modalSize("xl");
        $("#divModal").modal('show');
    });
}

$(Document).ready(function () {
    BindDealRequest(table.dtNewRequest, 1);

});

$('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
    var target = $(e.target).attr("href");// activated tab
    ActiveTab(target);
});

function ActiveTab(active) {
    switch (active) {
        case "#newrequest":
            BindDealRequest(table.dtNewRequest, 1);
            break;
        case "#approved":
            BindDealRequest(table.dtAccRequest, 2);
            break;
        case "#rejected":
            BindDealRequest(table.dtRejRequest, 3);
            break;
    }
}

function BindDealRequest(tableId, status) {
    table.id = tableId;
    if ($.fn.DataTable.isDataTable(table.id)) {
        $(table.id).dataTable().fnDestroy();


    }


    //DataTable
    dynamic = $(table.id).DataTable({
        "sAjaxSource": "/Distributor/ManageAdvertisement/GetDealOfTheDay?status=" + status,
        "initComplete": function (settings, json) {
            var api = new $.fn.dataTable.Api(settings);
            if (status !== 2)
                api.columns([1]).visible(false);
        },
        "rowCallback": function (row, data, index) {
            $(row).attr("data-RowId", data["id"]);
            if (status !== 2) {
                $(row).attr("data-RowId-OtherTab", data["id"]);
            }
            //Showing red color for expired medicine//

            // console.log("current date " + date);
            var date = new Date(Date());
            if (Date.parse(data.dealDate) < Date.parse(date)) {
                $('td', row).css('background-color', '#ff3333');
                $('td', row).css('color', 'white');
            }


        },

        "order": [[2, ""]],
        "language": {
            "info": "Showing _START_ to _END_ of _TOTAL_ Pharmacies",
            "sInfoEmpty": "Showing 0 to 0 of 0 Deal Request"
        },
        "columns": [
            {
                "data": "id",
                "autoWidth": false,
                "searchable": false,
                "orderable": false,
                "render": function (data, type, row) {
                    var content = "";
                    switch (status) {
                        case 1: content = `<a href="javascript:;" onclick=ViewRequest(${row.id}) class="dropdown-item action"><i class="fa fa-eye" aria-hidden="true"></i> View</a>
<a href="javascript:;" onclick=ViewRequest(${row.id}) class="dropdown-item action"><i class="fa fa-pencil" aria-hidden="true"></i> Edit</a>
<a href="javascript:;" onclick=DeleteRequest(${data}) class="dropdown-item action"><i class="fa fa-trash" aria-hidden="true"></i> Delete</a>`;

                            break;
                        case 2: content = ` <a href="javascript:;" onclick=ViewRequest(${row.id}) class="dropdown-item action"><i class="fa fa-eye" aria-hidden="true"></i> View</a>
<a href="javascript:;" onclick=ViewRequest(${row.id}) class="dropdown-item action"><i class="fa fa-pencil" aria-hidden="true"></i> Edit</a>
<a href="javascript:;" onclick=ViewRequest(${row.id}) class="dropdown-item action"><i class="fa fa-pencil" aria-hidden="true"></i> Delete</a>
<a href="javascript:;" onclick=DeleteRequest(${data}) class="dropdown-item action" > <i class="fa fa-envelope" aria-hidden="true"></i> Send Mail</a>`;
                            break;
                        case 3: content = `<a href="javascript:;" onclick=ViewRequest(${row.id}) class="dropdown-item action"><i class="fa fa-eye" aria-hidden="true"></i> View</a>
<a href="javascript:;" onclick=ViewRequest(${row.id}) class="dropdown-item action"><i class="fa fa-eye" aria-hidden="true"></i> Edit</a>
<a href="javascript:;" onclick=ViewRequest(${row.id}) class="dropdown-item action"><i class="fa fa-eye" aria-hidden="true"></i> Delete</a>
<a href="javascript:;" onclick=ViewRequest(${row.id}) class="dropdown-item action"><i class="fa fa-eye" aria-hidden="true"></i> Approve</a>`;
                            break;
                    }
                    return dataTableAction(content);
                }
            },
            {
                "data": "isActive",
                "autoWidth": false,
                "searchable": false,
                "orderable": false,
                "className": "admin_toggle",
                "render": function (data, type, row) {
                    return statusToggle(row.id, data);
                }
            },
            {
                "data": "requestNo",
                "autoWidth": false,
                "searchable": true
            },

            {
                "data": "dealDate",
                "autoWidth": false,
                "searchable": true
            },
            {
                "data": "request",
                "autoWidth": false,
                "searchable": true
            },
            {
                "data": "note",
                "autoWidth": false,
                "searchable": true
            }
        ]
    });


    $(table.id).on("change", ".admin_toggle", function (e) {

        const current = $(e.currentTarget).find("input");
        const id = $(current).data("id");

        $.ajax({
            url: table.SetIsActive + "/" + id,
            data: { id: id },
            type: "POST",
            success: function (response) {
                RxFair.HandleResponse(response);
            },
            error: function (data) {
            }
        });
    });
}

function DeleteRequest(id) {
    RxFair.Confirm("Delete Request", RxFair.typeModel.Warning, "Are you sure?", RxFair.typeModel.Danger, "Yes, delete it!", function () {
        $.ajax({
            url: table.DeleteClickAddress,
            data: { id: id },
            type: "GET",
            success: function (response) {
                RxFair.HandleResponse(response);
                if (response.status === 1) {
                    refreshDatatable(table.id, id);
                }
            },
            error: function (data) {
            }
        });
    }, function () { }
    );
}




