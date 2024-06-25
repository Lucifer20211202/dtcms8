<template>
	<div class="select-wrap">
		<div class="select-box">
			<el-tag size="large" effect="plain" disable-transitions closable
				@close="handleSelectClear" v-if="props.modelValue&&props.modelValue>0">
				{{datas.title}}
			</el-tag>
			<el-button icon="Plus" @click="handleOpenDialog" v-else>
				<span>{{props.placeholder}}</span>
			</el-button>
		</div>
	</div>
	
	<el-dialog v-model="datas.showDialog" title="选择会员" @close="handleDialogClose" style="min-width: 350px;" fullscreen append-to-body>
		<div class="select-wrap">
			<div class="search-box">
				<el-input placeholder="输入关健字" v-model="datas.keyword" @clear="initData" @keyup.enter.native="initData" clearable>
					<template #append>
						<el-button icon="Search" @click="initData"></el-button>
					</template>
				</el-input>
			</div>
			<div class="content-box">
				<el-card class="table-card">
					<el-table v-loading="datas.loading" :data="datas.listData" highlight-current-row @current-change="handleDialogChange" class="table-list">
						<el-table-column prop="userName" label="用户名" min-width="120"></el-table-column>
						<el-table-column prop="groupTitle" label="会员组" width="120"></el-table-column>
						<el-table-column label="状态" width="90">
							<template #default="scope">
								<el-tag type="success" effect="dark" v-if="scope.row.status==0">正常</el-tag>
								<el-tag type="warning" effect="dark" v-else-if="scope.row.status==1">待验证</el-tag>
								<el-tag type="warning" effect="dark" v-else-if="scope.row.status==2">待审核</el-tag>
								<el-tag type="info" effect="dark" v-else>黑名单</el-tag>
							</template>
						</el-table-column>
						<el-table-column prop="regTime" label="注册时间" width="160"></el-table-column>
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
	</el-dialog>
</template>

<script setup>
	import { ref,reactive,getCurrentInstance,watch,nextTick,onMounted } from "vue"
	//获取全局属性
	const { proxy } = getCurrentInstance()
	//通知回调
	const emits = defineEmits(['update:modelValue'])
	
	//接收props传值
	const props = defineProps({
		placeholder: {
			type: String,
			default: '请选择..'
		},
		modelValue: {
			type: Number,
			default: () => {
				return null
			}
		}
	})
	//定义变量
	const datas = reactive({
		loading: false,
		showDialog: false,
		isLoad: false,
		title: null,
		keyword: '',
		totalCount: 0,
		pageIndex: 1,
		pageSize: 10,
		listData: [],
	})
	
	//会员分页列表
	const initData = () => {
		let sendUrl = `/admin/member?status=0&pageSize=${datas.pageSize}&pageIndex=${datas.pageIndex}`
		if (datas.keyword.length > 0) {
			sendUrl += `&keyword=${encodeURI(datas.keyword)}`
		}
		proxy.$api.request({
			url: sendUrl,
			beforeSend() {
				datas.loading = true
			},
			success(res) {
				datas.listData = res.data
				let pageInfo = JSON.parse(res.headers["x-pagination"])
				datas.totalCount = pageInfo.totalCount
				datas.pageSize = pageInfo.pageSize
				datas.pageIndex = pageInfo.pageIndex
			},
			error(err) {
				datas.listData = []
			},
			complete () {
				datas.loading = false
			}
		})
	}
	//打开弹窗回调
	const handleOpenDialog = () => {
		datas.showDialog = true
		initData()
	}
	//关闭弹窗回调
	const handleDialogClose = () => {
		datas.keyword = ''
	}
	//选中回调
	const handleDialogChange = (val) => {
		if (!val) return
		datas.title = val.userName
		datas.showDialog = false
		//通知父组件更新
		emits('update:modelValue', val.userId)
	}
	//删除选中
	const handleSelectClear = () => {
		datas.title = null
		//通知父组件更新
		emits('update:modelValue', null)
	}
	//设置每页显示
	const handleSizeChange = (val) => {
		if (datas.pageSize != val) {
			datas.pageSize = val
			initData()
		}
	}
	//跳转第几页
	const handleCurrentChange = (val) => {
		 if (datas.pageIndex != val) {
			datas.pageIndex = val
			initData()
		}
	}
	
	//监听赋值的变化
	watch(() => props.modelValue, (newVal) => {
		if(newVal && newVal > 0) {
			if(datas.isLoad) return
			proxy.$api.request({
				url: `/admin/member/${newVal}`,
				success(res) {
					datas.title = res.data.userName
					datas.isLoad = true
				}
			})
		} else {
			datas.title = null
		}
	})
</script>