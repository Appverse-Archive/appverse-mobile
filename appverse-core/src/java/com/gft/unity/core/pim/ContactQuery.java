/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package com.gft.unity.core.pim;

/**
 *
 * @author FRMI
 */
public class ContactQuery {
    private ContactQueryColumn column;
    private ContactQueryCondition cond;
    private String value;

    public String getValue() {
        return value;
    }

    public void setValue(String value) {
        this.value = value;
    }        

    public ContactQuery() {
    }
       
    public ContactQueryColumn getColumn() {
        return column;
    }

    public void setColumn(ContactQueryColumn column) {
        this.column = column;
    }

    public ContactQueryCondition getCondition() {
        return cond;
    }

    public void setCondition(ContactQueryCondition cond) {
        this.cond = cond;
    }

    @Override
    public String toString() {
        return "ContactQuery{" + "column=" + column + ", cond=" + cond + ", value=" + value + '}';
    }

    
    
}
