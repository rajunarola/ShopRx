function ListPagesItem(sequence, pageNum, isCurrent, html) {
    this.sequence = sequence;
    this.pageNum = pageNum;
    this.isCurrent = isCurrent;
    this.html = html;
}

function listPages(allCount, numPerPage, currPage, pagerCount, allCurrStr, allLnkStr, currStr, lnkStr, prevStr, nextStr, mainPrevStr, mainNextStr) {
    var items = [];
    if (numPerPage <= 0) numPerPage = 1;
    var pagesCount = 1;
    while (allCount > numPerPage) {
        pagesCount++;
        allCount -= numPerPage;
    }
    if (pagesCount <= pagerCount) {
        if (currPage > 1)
            items.push(new window.ListPagesItem(items.Count, 0, 0 === currPage, mainPrevStr.replace(/#0#/g, (currPage - 1)).replace("cls", "prevEnable")));
        else
            items.push(new window.ListPagesItem(items.Count, 0, 0 === currPage, mainPrevStr.replace(/#0#/g, "0").replace("cls", "prevDisable")));


        for (var i = 1; i <= pagesCount; i++) {
            //items.push(new ListPagesItem(items.Count, i, i === currPage, i === currPage ? string.Format(currStr, i) : string.Format(lnkStr, i)));
            items.push(new window.ListPagesItem(items.Count, i, i === currPage, i === currPage ? currStr.replace(/#0#/g, i) : lnkStr.replace(/#0#/g, i)));
        }
        if (pagesCount > 1) {
            //items.push(new ListPagesItem(items.Count, 0, 0 === currPage, 0 === currPage ? string.Format(allCurrStr, 0) : string.Format(allLnkStr, 0)));
            //items.push(new ListPagesItem(items.Count, 0, 0 === currPage, 0 === currPage ? allCurrStr.replace(/#0#/g, 0) : allLnkStr.replace(/#0#/g, 0)));  //removed all
        }

        if (currPage < pagesCount)
            items.push(new window.ListPagesItem(items.Count, 0, 0 === currPage, mainNextStr.replace(/#0#/g, (currPage + 1)).replace("cls", "nextEnable")));
        else
            items.push(new window.ListPagesItem(items.Count, 0, 0 === currPage, mainNextStr.replace(/#0#/g, 0).replace("cls", "nextDisable")));
        return items;
    }
    var grCount = 0;
    var tmpPage = currPage === 0 ? 1 : currPage;
    while (tmpPage > grCount * pagerCount) {
        grCount++;
    }

    if (currPage > 1)
        items.push(new window.ListPagesItem(items.Count, 0, 0 === currPage, mainPrevStr.replace(/#0#/g, (currPage - 1)).replace("cls", "prevEnable")));
    else
        items.push(new window.ListPagesItem(items.Count, 0, 0 === currPage, mainPrevStr.replace(/#0#/g, "0").replace("cls", "prevDisable")));

    var tmpCurrPage = (grCount - 1) * pagerCount;
    if (grCount > 1) {
        //items.push(new ListPagesItem(items.Count, 1, false, string.Format(lnkStr, "1")));
        items.push(new ListPagesItem(items.Count, 1, false, lnkStr.replace(/#0#/g, "1")));

        //items.push(new ListPagesItem(items.Count, (tmpCurrPage - 1), false, string.Format(prevStr, "" + (tmpCurrPage - 1))));
        items.push(new ListPagesItem(items.Count, (tmpCurrPage - 1), false, prevStr.replace(/#0#/g, "" + (tmpCurrPage - 1))));
    }
    for (var i = (grCount - 1) * pagerCount; i <= grCount * pagerCount && i <= pagesCount; i++) {
        if (i !== 0)
            //items.push(new ListPagesItem(items.Count, i, i === currPage, i === currPage ? string.Format(currStr, i) : string.Format(lnkStr, i)));
            items.push(new ListPagesItem(items.Count, i, i === currPage, i === currPage ? currStr.replace(/#0#/g, i) : lnkStr.replace(/#0#/g, i)));
        tmpCurrPage++;
    }
    if (tmpCurrPage <= pagesCount) {
        //items.push(new ListPagesItem(items.Count, tmpCurrPage, false, string.Format(nextStr, tmpCurrPage)));
        items.push(new ListPagesItem(items.Count, tmpCurrPage, false, nextStr.replace(/#0#/g, tmpCurrPage)));

        //items.push(new ListPagesItem(items.Count, pagesCount, false, string.Format(lnkStr, "" + pagesCount)));
        items.push(new ListPagesItem(items.Count, pagesCount, false, lnkStr.replace(/#0#/g, "" + pagesCount)));
    }
    //items.push(new ListPagesItem(items.Count, 0, 0 === currPage, 0 === currPage ? string.Format(allCurrStr, 0) : string.Format(allLnkStr, 0)));
    //items.push(new ListPagesItem(items.Count, 0, 0 === currPage, 0 === currPage ? allCurrStr.replace(/#0#/g, 0) : allLnkStr.replace(/#0#/g, 0))); //removed all


    if (currPage < pagesCount)
        items.push(new ListPagesItem(items.Count, 0, 0 === currPage, mainNextStr.replace(/#0#/g, (currPage + 1)).replace("cls", "nextEnable")));
    else
        items.push(new ListPagesItem(items.Count, 0, 0 === currPage, mainNextStr.replace(/#0#/g, 0).replace("cls", "nextDisable")));
    return items;
}

function RenderPaginationResult(data) {
    var colorClass = {
        bestDeal:"green_border",
        contracted:"teal_border",
        previoslyPurchased:"yellow_border",
        shortDated:"red_border",
    };
    var isListView = $(".product-view-point").hasClass('list-view');
    var element = `<div class="${isListView ? '' : 'col-md-3'}">
            <div class="product-wrapper {MedicineColor}">
                <div class="product-img">
                    <a href="javascript:;">
                        <img src="/UploadFile/MedicineImage/{MedicineImage}" onerror="this.src='/rxfairbackend/images/placeholder.jpg'" class="w-100 product_img" >
                    </a>
                  <div class="cart-info">
                                <button  onclick="AddtoCart('{medicineId}','{distributorId}','{medPrice}')"><i class="fa fa-shopping-bag" aria-hidden="true"></i></button>
                   </div>
                 </div>
                <div class="product-text">
                    <div class="rating">
                    </div>
                    <h5 class="d-block product_text">{MedicineName}</h6>
                    <h5 class="d-block product_text">NDC : {NDC}</h7>
                    <h5 class="d-block product_text"><b>Dosage Form</b> : {DosageForm}</h7><br>
                    <h5 class="d-block product_text"><b>Packaging Size</b> : {PackageSize}</h7><br>
                    <h5 class="product-text-s product_text" data-toggle="tooltip" data-placement="top" title="{Manufacturer}"><b>Manufacturer</b> : {Manufacturer}</h7><br>
                    <h5 class="d-block product_text"><b>Strength</b> : {Strength}</h7>
                    <h5 class="d-block product_text">$ {MedicinePrice}</h5>
                    <p></p>
                </div>
            </div>
        </div>`;
    var content = "";
    if (data.length === 0) {
        content = `<div class="col-12">
                        <center><h3 class="text-center">No Medicine Found !</h3></center>
                </div>`;
    } else {
          
        var ElementData = "";
        if (data.length !== 0) {

            $.each(data, function (i, item) {
                var medicineData = element;
                
                    //assigning  Color accourding the conditions
                if (item.isCheap)
                    medicineData = medicineData.replace("{MedicineColor}", colorClass.previoslyPurchased);
                else {
                    // if 1 Medicine fullfiled more then 2 conditions
                    if (item.isShortDated && item.isContracted && item.isBestDeal)
                        medicineData = medicineData.replace("{MedicineColor}", colorClass.contracted);
                    //if 1 medicine fullfiled more then 1 conditions
                    if (item.isContracted && item.isBestDeal)
                        medicineData = medicineData.replace("{MedicineColor}", colorClass.contracted);
                    if (item.isShortDated && item.isBestDeal)
                        medicineData = medicineData.replace("{MedicineColor}", colorClass.bestDeal);
                    if (item.isShortDated && item.isContracted)
                        medicineData = medicineData.replace("{MedicineColor}", colorClass.contracted);
                    //if 1 medicine fullfiled only 1 conditions
                    if (item.isContracted) {
                        medicineData = medicineData.replace("{MedicineColor}", colorClass.contracted);
                    }
                    if (item.isShortDated) {
                        medicineData = medicineData.replace("{MedicineColor}", colorClass.shortDated);
                    }
                    if (item.isBestDeal) {
                        medicineData = medicineData.replace("{MedicineColor}", colorClass.bestDeal);
                    }
                }
                medicineData = medicineData.replace("{distributorId}", item.distributorId);
                medicineData = medicineData.replace("{medicineId}", item.id);
                medicineData = medicineData.replace("{MedicineImage}", item.medicineImage);
                medicineData = medicineData.replace("{MedicineName}", item.medicineName);
                medicineData = medicineData.replace("{medPrice}", item.wacPrice);
                medicineData = medicineData.replace("{NDC}", item.ndc);
                medicineData = medicineData.replace("{Strength}", item.strength);
                medicineData = medicineData.replace("{DosageForm}", item.dosageForm);
                medicineData = medicineData.replace("{PackageSize}", item.packageSize);
                medicineData = medicineData.replace("{Manufacturer}", item.manufacturerName == null ? "--" : item.manufacturerName);
                medicineData = medicineData.replace("{Manufacturer}", item.manufacturerName == null ? "--" : item.manufacturerName);
                medicineData = medicineData.replace("{MedicinePrice}", item.wacPrice == 0 ? "Price Is Not Available" : item.wacPrice);
                ElementData = ElementData + medicineData;
            });
            content = ElementData;
        }
    }
    return content;
}