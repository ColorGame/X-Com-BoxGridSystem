


using System;

public struct GridPosition : IEquatable<GridPosition> //��������� Equatable - ��������������    //������ � �������� ��������� ������������ ��� ���� ������ �������� ����������� ����� ������ � C#. ����� ���� ������ ����������� ����, ��������, int, double � �.�.,
                                                                                                //�� ���� �������� �����������. ��� ������ � ���������� �� �������� ����� �������� � �� ������.
                                                                                                //�� ������� ���� ��������� �.�. �� ����� ��������������� ����������� Vector2Int. �� �������� � X Y � ��� ����� X Z (����� ���� �� ������� �������������� �� � XZ ���� � �������, �� ��� ��������� ����� ����� ���� � ���� ��������������)
{
    public int x;
    public int z;

    public GridPosition(int x, int z) // ��������������� �����������
    {
        this.x = x;
        this.z = z;
    }
    public override string ToString() // ������������� ToString(). ����� ������� � ������� Debug.Log ��������� ��������� X Z
    {
        return $"x: {x}; z: {z}";
    }



    public static bool operator == (GridPosition a, GridPosition b) // ���������� ��� ������� �������� ���������
    {
        return a.x==b.x && a.z==b.z ;
    }

    public static bool operator != (GridPosition a, GridPosition b) // ���������� ��� ������� �������� ���������
    {
        return !(a == b);
    }

    public static GridPosition operator + (GridPosition a, GridPosition b) // ���������� ��� �����
    {
        return new GridPosition(a.x + b.x, a.z + b.z);
    }
    
    public static GridPosition operator - (GridPosition a, GridPosition b) // ���������� ��� ��������
    {
        return new GridPosition(a.x - b.x, a.z - b.z);
    }

    public override bool Equals(object obj) // ������������� ��������������� ���������� ��������������� ���������
    {
        return obj is GridPosition position &&
               x == position.x &&
               z == position.z;
    }

    public override int GetHashCode() // ������������� ��������������� ����������
    {
        return HashCode.Combine(x, z);
    }

    public bool Equals(GridPosition other) // ���������� ���������� ���������
    {
        return this == other;
    }
}