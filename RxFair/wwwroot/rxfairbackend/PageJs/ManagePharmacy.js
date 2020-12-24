var dynamic;
var table = {
    id: "",
    dtPenPharmacy: "#dtPendingPharmacy",
    dtAccPharmacy: "#dtAcceptedPharmacy",
    dtRejPharmacy: "#dtRejectedPharmacy",

    ViewClickAddress: "/Admin/ManagePharmacy/ViewPharmacy?id=",
    DeleteClickAddress: "/Admin/ManagePharmacy/RemovePharmacy",
    StatusAddress: "/Admin/ManagePharmacy/ManagePharmacyStatus",
    SetIsActive: "/Admin/ManagePharmacy/ManageIsActive"
};
var custom = {
    "sLengthMenu": "Display _MENU_ records per page",
    "sZeroRecords": "Nothing found - sorry",
    "sInfo": "Showing _START_ to _END_ of _TOTAL_ records" + "Changes",
    "sInfoEmpty": "Showing 0 to 0 of 0 records",
    "sInfoFiltered": "(filtered from _MAX_ total records)"
};

function ViewPharmacyAccount(id) {
    $("#modalContent").load(`/Admin/ManagePharmacy/ViewPharmacyAccount/${id}`, function () {
      modalSize("xl");
        $("#divModal").modal('show');
    });
}

$(Document).ready(function () {
    var active = getUrlFields(window.location.href, 6);
    if (active !== "" && (active === "Active" || active === "New")) {
        if (active === "Active") {
            ActiveTab("#AcceptedTab", true);
        } else {
            ActiveTab("#PendingTab", true);
        }
    } else {
        BindPharmacyRequest(table.dtPenPharmacy, 1);
    }
});

function SetPharmacyStatus(pharmacyId, status) {
    RxFair.Confirm("Change Pharmacy Status", RxFair.typeModel.Warning, "Are you sure?", RxFair.typeModel.Danger, "Yes, change it!", function () {
        $.ajax({
            url: table.StatusAddress + "?id=" + pharmacyId + "&status=" + status,
            type: 'POST',
            success: function (response) {
                $(table.id).DataTable().row("[data-RowId='" + pharmacyId + "']").remove().draw(false);
                RxFair.HandleResponse(response);
                //"Pharmacy " + status === 2 ? + "accepted" : "rejected" + " successfully."
            }
        });
    }, function () { }
    );
}

$('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
    var target = $(e.target).attr("href");// activated tab
    ActiveTab(target);
});

function ActiveTab(active, isShow = false) {
    if (isShow) {
        $('#myTab1 a[href="' + active + '"]').tab('show');
    }
    switch (active) {
        case "#PendingTab":
            BindPharmacyRequest(table.dtPenPharmacy, 1);
            break;
        case "#AcceptedTab":
            BindPharmacyRequest(table.dtAccPharmacy, 2);
            break;
        case "#RejectedTab":
            BindPharmacyRequest(table.dtRejPharmacy, 3);
            break;
    }

}

function BindPharmacyRequest(tableId, status) {
    table.id = tableId;

    if ($.fn.DataTable.isDataTable(table.id)) {
        $(table.id).dataTable().fnDestroy();
    }

    //DataTable
    dynamic = $(table.id).DataTable({
        "sAjaxSource": "/Admin/ManagePharmacy/GetPharmacyRequestList?status=" + status,
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
        },
        "order": [[2, ""]],
        "language": {
            "info": "Showing _START_ to _END_ of _TOTAL_ Pharmacies",
            "sInfoEmpty": "Showing 0 to 0 of 0 Pharmacies"
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
                        case 1:
                            if (row.emailConfirmed) {
                                content = `<a href="javascript:;" onclick=SetPharmacyStatus(${data},2) class="dropdown-item action"><i class="fa fa-check-circle-o" aria-hidden="true"></i> Accept</a>
                               <a href="javascript:;" onclick=SetPharmacyStatus(${data},3) class="dropdown-item action"><i class="fa fa-window-close-o" aria-hidden="true"></i> Reject</a>`;
                            }
                            break;
                        case 2: content = ``;
                            break;
                        case 3:
                            if (row.emailConfirmed) {
                                content = `<a href="javascript:;" onclick=SetPharmacyStatus(${data},2) class="dropdown-item action"><i class="fa fa-check-circle-o" aria-hidden="true"></i> Accept</a>`;
                            }
                            break;
                    }
                    //content = content + `<a href="javascript:;" onclick=DeletePharmacy(${data}) class="dropdown-item action"><i class="fa fa-trash" aria-hidden="true"></i> Delete</a>`;
                    content = content + `<a href="javascript:;" onclick=ViewPharmacyAccount(${row.id}) class="dropdown-item action"><i class="fa fa-eye" aria-hidden="true"></i> View</a>`;
                    return dataTableAction(content);
                }
            },
            {
                "data": "isActive",
                "autoWidth": false,
                "searchable": false,
                "orderable": false,
                "className": "admin_toggle",
                "render": function (data, type, row){
                    return statusToggle(row.id, data);
                }
            },
            {
                "data": "emailConfirmed",
                "autoWidth": true,
                "searchable": false,
                "orderable": false,
                "render": function (data, type, row) {
                    return emailConfirmed(data);
                }
            },
            {
                "data": "pharmacyName",
                "autoWidth": false,
                "searchable": true
            },
            {
                "data": "pharmacyTypeName",
                "autoWidth": false,
                "searchable": true
            },
            {
                "data": "jobTitle",
                "autoWidth": false,
                "searchable": true
            },
            {
                "data": "primaryEmail",
                "autoWidth": false,
                "searchable": true
            },
            {
                "data": "phoneNumber",
                "autoWidth": false,
                "searchable": true
            }
            //,{
            //  "data": "billAddress1",
            //  "autoWidth": false,
            //  "searchable": true
            //}
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

function DeletePharmacy(id) {
    RxFair.Confirm("Delete Pharmacy", RxFair.typeModel.Warning, "Are you sure?", RxFair.typeModel.Danger, "Yes, delete it!", function () {
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
