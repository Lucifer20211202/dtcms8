<template>
	<div class="mainbody">
		<dt-location :data="[{title:'文章管理'},{title:'内容列表'}]"></dt-location>
		<dt-toolbar-box>
			<template #default>
				<div class="list-box">
					<div class="l-list">
						<el-button-group>
							<el-button icon="Plus" @click="$common.linkUrl(`/article/edit/${props.channelId}`)">新增</el-button>
							<el-button icon="Finished" @click="auditCheckAll">审核</el-button>
							<el-button icon="CircleCheck" v-if="datas.showImgList" @click="toggleCheckAll">
								<template v-if="!datas.checkAll">全选</template>
								<template v-else>取消</template>
							</el-button>
							<el-button icon="Delete" @click="deleteCheckAll">删除</el-button>
						</el-button-group>
					</div>
					<div class="r-list">
						<div class="search-box">
							<el-input placeholder="输入关健字" v-model="datas.keyword" @clear="initData" @keyup.enter.native="initData" clearable>
								<template #append>
									<el-button icon="Search" @click="initData"></el-button>
								</template>
							</el-input>
						</div>
						<div class="btn-box">
							<el-button-group>
								<el-button @click="toggleImgList">
									<template #default>
										<i :class="datas.iconImgList"></i>
									</template>
								</el-button>
							</el-button-group>
						</div>
					</div>
				</div>
			</template>
			<template #more>
				<div class="more-box">
					<dl>
						<dt>筛选类别</dt>
						<dd>
							<el-select v-model="datas.categoryId" @change="initData" placeholder="请选择...">
								<el-option :key="0" label="所有类别" :value="0"></el-option>
								<el-option v-for="item in datas.categoryList"
									:key="item.id"
									:label="item.title"
									:value="item.id">
								</el-option>
							</el-select>
						</dd>
					</dl>
					<dl>
						<dt>标签筛选</dt>
						<dd>
							<el-select v-model="datas.labelId" @change="initData" placeholder="请选择...">
								<el-option key="0" label="不限标签" :value="0"></el-option>
								<el-option v-for="item in datas.labelList"
									:key="item.id"
									:label="item.title"
									:value="item.id">
								</el-option>
							</el-select>
						</dd>
					</dl>
					<dl>
						<dt>发布日期</dt>
						<dd>
							<el-date-picker style="width:280px" v-model="datas.dateRange"
								type="daterange"
								value-format="YYYY-MM-DD"
								range-separator="至"
								start-placeholder="开始时间"
								end-placeholder="结束时间"
								@change="initData">
							</el-date-picker>
						</dd>
					</dl>
				</div>
			</template>
		</dt-toolbar-box>
		
		<div class="content-box">
			<!--图片列表-->
			<div class="img-list" v-loading="datas.loading" v-if="datas.showImgList">
				<el-card v-if="datas.listData.length==0" class="nodata">暂无数据...</el-card>
				<el-card v-for="(item,index) in datas.listData" :key="index">
					<div class="lock" v-if="item.status==1">审核中</div>
					<div class="check">
						<el-checkbox v-model="item.checked"></el-checkbox>
					</div>
					<el-image class="pic" fit="cover" :src="$api.loadFile(item.imgUrl)" :preview-src-list="[$api.loadFile(item.imgUrl)]">
						<template #error>
							<div class="image-slot">
								<el-icon><Picture /></el-icon>
							</div>
						</template>
					</el-image>
					<i class="absbg"></i>
					<h1>
						<span><a @click="$common.linkUrl(`/article/edit/${props.channelId}/${item.id}`)">{{item.title}}</a></span>
					</h1>
					<div class="remark">
						<span>{{item.zhaiyao}}</span>
					</div>
					<div class="tools">
						<el-tag v-for="(citem,i) in item.categoryTitle.split(',')" :key="i">
							{{ citem }}
						</el-tag>
					</div>
					<div class="foot">
						<p class="time">{{item.addTime}}</p>
						<span>
							<el-link :underline="false" title="复制" icon="DocumentCopy" @click="$common.linkUrl(`/article/edit/${props.channelId}`, {copyId: item.id})"></el-link>
							<el-link :underline="false" title="编辑" icon="Edit" @click="$common.linkUrl(`/article/edit/${props.channelId}/${item.id}`)"></el-link>
							<el-link :underline="false" title="删除" icon="Delete" @click="deleteItem(item.id)"></el-link>
						</span>
					</div>
				</el-card>
			</div>
			<!--文字列表-->
			<el-card class="table-card" v-else>
				<el-table ref="tableRef" v-loading="datas.loading" :data="datas.listData" stripe class="table-list" @selection-change="handleSelectionChange">
					<el-table-column type="selection" width="45"></el-table-column>
					<el-table-column label="标题" min-width="220">
						<template #default="scope">
							<el-image class="pic" fit="contain" :src="$api.loadFile(scope.row.imgUrl)" :preview-src-list="[$api.loadFile(scope.row.imgUrl)]">
								<template #error>
									<div class="image-slot">
										<el-icon><Picture /></el-icon>
									</div>
								</template>
							</el-image>
							<h4>{{scope.row.title}}</h4>
							<span class="date">
								<el-icon><Calendar /></el-icon>
								{{scope.row.addTime}}
							</span>
						</template>
					</el-table-column>
					<el-table-column label="所属类别" min-width="160">
						<template #default="scope">
							<div class="nowrap">
								<el-tag size="small" v-for="(item,index) in scope.row.categoryTitle.split(',')"
									:key="index">
									{{ item }}
								</el-tag>
							</div>
						</template>
					</el-table-column>
					<el-table-column prop="click" label="浏览量" width="90"></el-table-column>
					<el-table-column prop="status" label="状态" width="80" align="center">
						<template #default="scope">
							<el-tag size="small" type="success" effect="dark" v-if="scope.row.status==0">正常</el-tag>
							<el-tag size="small" type="warning" effect="dark" v-else-if="scope.row.status==1">待审</el-tag>
							<el-tag size="small" type="warning" effect="dark" v-else-if="scope.row.status==2">已删</el-tag>
						</template>
					</el-table-column>
					<el-table-column label="排序" width="120" align="center">
						<template #default="scope">
							<el-input-number size="small" v-model="scope.row.sortId"
								@change="updateField(scope.row.id, '/sortId', scope.row.sortId)" :min="-99999999" :max="99999999" />
						</template>
					</el-table-column>
					<el-table-column fixed="right" label="操作" width="120">
						<template #default="scope">
							<el-button size="small" icon="DocumentCopy" @click="$common.linkUrl(`/article/edit/${props.channelId}`, {copyId: item.id})"></el-button>
							<el-button size="small" icon="Edit" @click="$common.linkUrl(`/article/edit/${props.channelId}/${scope.row.id}`)"></el-button>
							<el-button size="small" type="danger" icon="Delete" @click="deleteItem(scope.row.id)"></el-button>
						</template>
					</el-table-column>
				</el-table>
			</el-card>
			
			<div class="pager-box">
				<el-pagination background
					@size-change="handleSizeChange"
					@current-change="handleCurrentChange"
					:current-page="datas.pageIndex"
					:page-sizes="[10, 20, 50, 100]"
					:page-size="datas.pageSize"
					layout="total, sizes, prev, pager, next, jumper"
					:total="datas.totalCount">
				</el-pagination>
			</div>
		</div>
	</div>
</template>

<script setup>
	import { ref,reactive,getCurrentInstance,nextTick,watch } from "vue"
	import DtToolbarBox from '../../components/layout/DtToolbarBox.vue'
	
	//获取全局属性
	const { proxy } = getCurrentInstance()
	const tableRef = ref(null)
	
	//接收props传值
	const props = defineProps({
		config: {
			type: Object,
			default: {}
		},
		channelId: 0
	})
	
	//定义属性
	const datas = reactive({
		loading: false,
		showImgList: true, //显示图片列表
		iconImgList: 'iconfont icon-list-txt',//切换按钮图标
		checkAll: false, //是否已全选
		categoryId: 0,
		labelId: 0,
		wxAccountId: null,
		dateRange: [],
		categoryList: [],
		labelList: [],
		wxAccountList: [],
		keyword: '',
		totalCount: 0,
		pageSize: 10,
		pageIndex: 1,
		listData: [],
		multipleSelection: [],
	})
	
	//初始化数据
	const initData = async() => {
		let sendUrl = `/admin/article/${props.channelId}?pageSize=${datas.pageSize}&pageIndex=${datas.pageIndex}`
		sendUrl += `&categoryId=${datas.categoryId}&labelId=${datas.labelId}`
		if (datas.dateRange && datas.dateRange.length > 0) {
			sendUrl += `&startDate=${datas.dateRange[0]}&endDate=${datas.dateRange[1]}`
		}
		if (datas.keyword.length > 0) {
			sendUrl += `&keyword=${encodeURI(datas.keyword)}`
		}
		//获取分页列表
		await proxy.$api.request({
			url: sendUrl,
			beforeSend() {
				datas.loading = true
			},
			success(res) {
				datas.listData = res.data;
				let pageInfo = JSON.parse(res.headers["x-pagination"])
				datas.totalCount = pageInfo.totalCount
				datas.pageSize = pageInfo.pageSize
				datas.pageIndex = pageInfo.pageIndex
				datas.listData.forEach(row => {
					row.checked = false //默认不选中
				})
			},
			error(err) {
				datas.listData = []
			},
			complete() {
				datas.loading = false
			}
		})
	}
	//初始化工具栏
	const initLoad = () => {
		//加载分类列表
		proxy.$api.request({
			url: `/admin/article/category/${props.channelId}`,
			success(res) {
				datas.categoryList = res.data
			}
		})
		//加载标签
		proxy.$api.request({
			url: `/admin/article/label/view/${props.channelId}/100?status=0`,
			success(res) {
				datas.labelList = res.data
			}
		})
	}
	//切换图文列表方式
	const toggleImgList = () => {
		datas.showImgList = !datas.showImgList
		if(datas.showImgList) {
			datas.iconImgList = 'iconfont icon-list-txt'
		} else {
			datas.iconImgList = 'iconfont icon-list-img'
		}
	}
	//全选切换
	const toggleCheckAll = () => {
		if (datas.listData) {
			datas.listData.forEach((item, index) => {
				if (!datas.checkAll) {
					item.checked = true
				} else {
					item.checked = false
				}
			})
		}
		datas.checkAll = !datas.checkAll
	}
	//批量审核
	const auditCheckAll = () => {
		//拿到选中的数据
		let listIds = [] //创建一个数组
		if (datas.listData) {
			datas.listData.forEach((item, index) => {
				if(item.checked) {
					listIds.push(item.id)
				}
			})
		}
		//检查是否有选中
		if (!listIds.length) {
			proxy.$message({ type: 'warning', message: '请选择要审核的记录' })
			return false
		}
		//执行操作
		proxy.$confirm('确认批量审核记录吗？', '提示', {
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning'
		}).then(() => {
			proxy.$api.request({
				method: 'put',
				url: `/admin/article/${props.channelId}?ids=${listIds.toString()}`,
				loading: true,
				successMsg: '批量审核完成',
				success(res) {
					initData() //重新加载列表
				}
			});
		}).catch(() => { })
	}
	//多选删除
	const deleteCheckAll = () => {
		//拿到选中的数据
		let listIds = [] //创建一个数组
		if (datas.listData) {
			datas.listData.forEach((item, index) => {
				if(item.checked) {
					listIds.push(item.id)
				}
			})
		}
		//检查是否有选中
		if (!listIds.length) {
			proxy.$message({ type: 'warning', message: '请选择要删除的记录' })
			return false
		}
		//执行删除操作
		proxy.$confirm('确认要删除该记录吗？', '提示', {
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning'
		}).then(() => {
			proxy.$api.request({
				method: 'delete',
				url: `/admin/article/${props.channelId}?ids=${listIds.toString()}`,
				loading: true,
				successMsg: '已删除成功',
				success(res) {
					initData() //重新加载列表
				}
			})
		}).catch(() => { })
	}
	//单项删除
	const deleteItem = (val) => {
		//执行删除操作
		proxy.$confirm('确认要删除该记录吗？', '提示', {
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning'
		}).then(() => {
			proxy.$api.request({
				method: 'delete',
				url: `/admin/article/${props.channelId}/${val}`,
				loading: true,
				successMsg: '已删除成功',
				success(res) {
					initData() //重新加载列表
				}
			})
			
		}).catch(() => { })
	}
	//选中第几行
	const handleSelectionChange = (val) => {
		datas.listData.forEach((item) => {
			item.checked = false
			val.forEach((x) => {
				if (item.id == x.id) {
					item.checked = true
				}
			})
		})
	}
	//每页显示数量
	const handleSizeChange = (val) => {
		if (datas.pageSize != val) {
			datas.pageSize = val
			initData()
		}
	}
	//跳转到第几页
	const handleCurrentChange = (val) => {
		if (datas.pageIndex != val) {
			datas.pageIndex = val
			initData()
		}
	}
	//修改部分字段
	const updateField = (id, path, val) => {
		proxy.$api.request({
			method: 'patch',
			url: `/admin/article/${props.channelId}/${id}`,
			data: [{ "op": "replace", "path": path, "value": val }],
			success(res) { }
		})
	}
	
	//监听页面切换初始化Table选中的复选框
	watch(() => datas.showImgList, (newVal, oldVal) => {
		if (!newVal && datas.listData) {
			datas.listData.forEach((row) => {
				if (row.checked) {
					//加上nextTick是为了等待组件加载完毕，否则出错
					nextTick(() => {
						tableRef.value.toggleRowSelection(row)
					})
				}
			});
		}
	})
	
	//执行初始化方法
	initData()
	//初始化工具栏
	initLoad()
</script>