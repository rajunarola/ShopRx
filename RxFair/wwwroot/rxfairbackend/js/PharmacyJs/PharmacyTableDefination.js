var table = {
    id:"",
    dtPenPharmacy: "#penPharmacy",
    dtAccPharmacy: "#acpPharmacy",
    dtRejPharmacy: "#rejPharmacy",

    ViewClickAddress: "ManagePharmacies/ViewPharmacy?id=",
    DeleteClickAddress: "ManagePharmacies/DeletePharmacy",
    StatusAddress: "ManagePharmacies/SetPharmacyStatus"
};

function SetPharmacyStatus(PharmacyID, status) {
    $.ajax({
        url: table.StatusAddress + "?id=" + PharmacyID + "&status=" + status,
        type: 'POST',
        success: function () {
            dynamic.row('.selected').remove().draw(false);
        }
    });
}
function DeletePharmacy(PharmacyID) {
    $.ajax({
        url: table.DeleteClickAddress + "?id=" + PharmacyID,
        type: 'POST',
        success: function () {
        }
    });
}

var dynamic;
$('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {    
    var target = $(e.target).attr("id");// activated tab
     ActiveTab(target);
});

function ActiveTab(active) {
    switch (active) {
        case "PendingTab":
            setTable(table.dtPenPharmacy, 1);
            break;
        case "AcceptedTab":
            setTable(table.dtAccPharmacy, 2);
            break;
        case "RejectedTab":
            setTable(table.dtRejPharmacy, 3);
            break;
    }
}

function setTable(TableId, Status) {
    table.id = TableId;
    //$(this).data("id")
    //$(this).attr("data-id")
    var accept = '<a class="dropdown-item" href="javascript:void(0);" onclick=SetPharmacyStatus({pharmacyid},2)><i class="fa fa- check-circle-o" aria-hidden="true"></i>Accept</a>';
    var reject = '<a class="reject-action dropdown-item" href="javascript:void(0);" onclick=SetPharmacyStatus({pharmacyid},3)><i class="fa fa-window-close-o" aria-hidden="true"></i>Reject</a>';
    var del = '<a class="dropdown-item" href="javascript:void(0);"  onclick=DeletePharmacy({pharmacyid}) ><i class="fa fa-trash" aria-hidden="true"></i>Delete</a>';
    var view = '<a class="dropdown-item" href="javascript:void(0);"><i class="fa fa-eye" aria-hidden="true"></i>View</a>';
    var toggle = ' <td class="admin_toggle">\
                        < label class="switch" >\
                           <input type="checkbox" checked>\
                                    <span class="slider round"></span>\
                                        </label>\
                                    </td >';

    var actionMenuHtml = '<td class="data_optionsec">\
                            <div class="">\
                                <button type="button" class="btn dropdown-toggle admin_detail" data-toggle="dropdown">\
                                    <i class="fa fa-circle-o" aria-hidden="true"></i>\
                                    <i class="fa fa-circle-o" aria-hidden="true"></i>\
                                    <i class="fa fa-circle-o" aria-hidden="true"></i>\
                                </button>\
                                <div class="dropdown-menu">{actions}\
                                </div>\
                            </div>\
                         </td>';

    if ($.fn.DataTable.isDataTable(TableId)) {
        $(TableId).dataTable().fnDestroy();
    }
    
    dynamic = $(TableId).DataTable({
        ajax: "ManagePharmacies/getPharmacyList?status=" + Status,
        columns: [
            {
                data:"id",
                className: "data_optionsec",
                render: function (data, type, row) {
                    var tdActionMenu = actionMenuHtml;
                    var actions = "";
                    if (row.status === 1){
                        actions = (view + accept + reject + del).replace(/{pharmacyid}/g, row.id);
                    } else {
                        actions = (view + del).replace(/{pharmacyid}/g, row.id);
                    }
                    tdActionMenu = tdActionMenu.replace("{actions}", actions);
                    return tdActionMenu;     
                }
            },
            {
                data: "id",
                className: "admin_toggle",
                render: function () {
                    return toggle;
                }
            },
            { data: "pharmacyName" },
            { data: "pharmacyTypeName" },
            { data: "jobTitle" },
            { data: "primaryEmail" },
            { data: "phoneNumber" },
            { data: "billAddress1" }
        ]
    });

    function ViewPharmacy(e) {
        console.log(e);
    }
}

$(document).ready(function () {
    setTable(table.dtPenPharmacy, 1);
    // Apply the search
    dynamic.columns().eq(0).each(function (colIdx) {
        $('input', table.column(colIdx).header()).on('keyup change', function () {
            table
                .column(colIdx)
                .search(this.value)
                .draw();
        });
    });
});