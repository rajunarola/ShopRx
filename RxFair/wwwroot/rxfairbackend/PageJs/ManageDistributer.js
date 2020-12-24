//  Option Menu Functions
function AddEditDistributor(id) {
    if (id === 0)
        window.location.href = "/Admin/ManageDistributor/AddEditDistributor/";
    else
        window.location.href = "/Admin/ManageDistributor/AddEditDistributor/" + id;
}
function ViewDistributer(id) {

    window.location.href = "/Admin/Managedistributor/ViewDistributer/" + id;
}

function DeleteDistributer(id) {
    RxFair.Confirm("Delete Distributor", RxFair.typeModel.Warning, "Are you sure?", RxFair.typeModel.Danger, "Yes, delete it!", function () {
        $.ajax({
            url: "/Admin/ManageDistributor/RemoveDistributer",
            data: { id: id },
            type: "GET",
            success: function (response) {
                RxFair.HandleResponse(response);
                if (response.status === 1) {
                    //$(table.id).DataTable().row("[data-RowId='" + response.data + "']").remove().draw(false);
                    refreshDatatable(table.id, id);
                }
            },
            error: function (data) {
            }
        });
    }, function () { }
    );
}
$(document).ready(function () {
    BindManageDistributerList();

});

function ViewEditOrderSetting(id) {
    $("#modalContent").load(`/Admin/ManageDistributor/EditOrderSetting/${id}`, function () {
        modalSize("lg");
        $("#divModal").modal('show');
    });
}
function BindManageDistributerList() {
    if ($.fn.DataTable.isDataTable("#dtManageDistributer")) {
        $("#dtManageDistributer").DataTable().destroy();
    }
    table = $("#dtManageDistributer")
        .DataTable({
            "sAjaxSource": "/Admin/ManageDistributor/GetDistributerList",
            "rowCallback": function (row, data, index) {
                $(row).attr("data-RowId", data["id"]);
            },
            "order": [[2, ""]],
            "language": {
                "info": "Showing _START_ to _END_ of _TOTAL_ Distributers",
                "sInfoEmpty": "Showing 0 to 0 of 0 Distributers"
            },
            "columns": [
                {
                    "data": "id",
                    "autoWidth": false,
                    "searchable": false,
                    "orderable": false,
                    "render": function (data, type, row) {
                        var content = `<a href="javascript:;" onclick=ViewDistributer(${data}) class="dropdown-item action"><i class="fa fa-eye" aria-hidden="true"></i> View</a>
                           <a href="javascript:;" onclick=AddEditDistributor(${data}) class="dropdown-item action"><i class="fa fa-pencil" aria-hidden="true"></i> Edit Distributor</a>
                           <a href="javascript:;" onclick=ViewEditOrderSetting(${data}) class="dropdown-item action"><i class="fa fa-shopping-cart" aria-hidden="true"></i> Edit Order Setting</a>`;
                        //<a href="javascript:;" onclick=DeleteDistributer(${data}) class="dropdown-item action"><i class="fa fa-trash" aria-hidden="true"></i> Delete</a>
                        return dataTableAction(content);
                    }
                },
                {
                    "data": "isActive",
                    "autoWidth": false,
                    "searchable": false,
                    "orderable": false,
                    "className": "admin_toggle text-center",
                    "render": function (data, type, row) {
                        return statusToggle(row.id, data);
                    }
                },
                {
                    "data": "emailConfirmed",
                    "autoWidth": false,
                    "searchable": false,
                    "orderable": false,
                    "render": function (data, type, row) {
                        return emailConfirmed(data);
                    }
                },
                {
                    "data": "companyName",
                    "autoWidth": false,
                    "searchable": true
                },
                {
                    "data": "email",
                    "autoWidth": false,
                    "searchable": true
                },
                {
                    "data": "mobile",
                    "autoWidth": false,
                    "searchable": true
                },
                {
                    "data": "contactAddress",
                    "autoWidth": false,
                    "searchable": true
                }

            ]
        });

    $("#dtManageDistributer").on("change", ".admin_toggle", function (e) {

        const current = $(e.currentTarget).find("input");
        const id = $(current).data("id");

        $.ajax({
            url: "/Admin/ManageDistributor/ManageIsActive",
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
